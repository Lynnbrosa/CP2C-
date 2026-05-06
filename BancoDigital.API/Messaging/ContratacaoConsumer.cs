using BancoDigital.API.Data;
using BancoDigital.API.Entities;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BancoDigital.API.Messaging;

public class ContratacaoConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContratacaoConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private const string QueueName = "contratacoes";

    public ContratacaoConsumer(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ContratacaoConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    var payload = JsonSerializer.Deserialize<MensagemContratacao>(json);
                    if (payload != null)
                        await ProcessarAsync(payload.ContratacaoId);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar contratação: {json}", json);
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            _logger.LogInformation("Consumer RabbitMQ iniciado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Não foi possível conectar ao RabbitMQ. Consumer não iniciado.");
        }

        return Task.CompletedTask;
    }

    private async Task ProcessarAsync(int contratacaoId)
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var contratacao = await ctx.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == contratacaoId);

        if (contratacao is null) return;

        contratacao.Status = StatusContratacao.EmAnalise;
        await ctx.SaveChangesAsync();

        await Task.Delay(300); // simula latência de análise

        switch (contratacao.Produto)
        {
            case Emprestimo emp:
                AplicarRegraEmprestimo(contratacao, emp);
                break;
            case MaquinaDeCartao mdc:
                AplicarRegraMaquinaDeCartao(contratacao, mdc);
                break;
            default:
                contratacao.Status = StatusContratacao.Aprovada;
                contratacao.Observacao = "Aprovado automaticamente.";
                break;
        }

        contratacao.DataProcessamento = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
        _logger.LogInformation("Contratação {Id} processada: {Status}", contratacaoId, contratacao.Status);
    }

    // Regra de negócio: score = (idade - 18) * 5 + renda / 500
    private void AplicarRegraEmprestimo(Contratacao contratacao, Emprestimo emp)
    {
        int score = CalcularScore(contratacao.Cliente);
        contratacao.Score = score;

        if (contratacao.ValorSolicitado is null || contratacao.PrazoMeses is null)
        {
            contratacao.Status = StatusContratacao.Reprovada;
            contratacao.Observacao = "Valor e prazo são obrigatórios para empréstimo.";
            return;
        }

        if (contratacao.ValorSolicitado < emp.ValorMinimo || contratacao.ValorSolicitado > emp.ValorMaximo)
        {
            contratacao.Status = StatusContratacao.Reprovada;
            contratacao.Observacao = $"Valor fora do intervalo permitido ({emp.ValorMinimo:C} – {emp.ValorMaximo:C}).";
            return;
        }

        if (contratacao.PrazoMeses > emp.PrazoMaximoMeses)
        {
            contratacao.Status = StatusContratacao.Reprovada;
            contratacao.Observacao = $"Prazo máximo é {emp.PrazoMaximoMeses} meses.";
            return;
        }

        if (score >= emp.ScoreMinimo)
        {
            contratacao.Status = StatusContratacao.Aprovada;
            contratacao.Observacao = $"Aprovado. Score: {score}. Taxa: {emp.TaxaJuros}% a.m.";
        }
        else
        {
            contratacao.Status = StatusContratacao.Reprovada;
            contratacao.Observacao = $"Score insuficiente: {score} (mínimo {emp.ScoreMinimo}).";
        }
    }

    // Regra de negócio: taxa MDR variável por ramo de atividade da PJ
    private void AplicarRegraMaquinaDeCartao(Contratacao contratacao, MaquinaDeCartao mdc)
    {
        decimal taxa = contratacao.Cliente is PessoaJuridica pj
            ? CalcularTaxaMdr(pj.RamoAtividade, mdc.TaxaMdrBase)
            : mdc.TaxaMdrBase;

        contratacao.TaxaMdrAplicada = taxa;
        contratacao.Status = StatusContratacao.Aprovada;
        contratacao.Observacao = $"Aprovado. Modelo: {mdc.Modelo}. Taxa MDR: {taxa:P2}.";
    }

    private static int CalcularScore(Cliente cliente)
    {
        if (cliente is PessoaFisica pf)
        {
            int idade = DateTime.Today.Year - pf.DataNascimento.Year;
            if (pf.DataNascimento.Date > DateTime.Today.AddYears(-idade)) idade--;
            return (int)((idade - 18) * 5 + pf.Renda / 500m);
        }
        return 400; // PJ recebe score fixo base
    }

    private static decimal CalcularTaxaMdr(string? ramo, decimal taxaBase) =>
        ramo?.ToLowerInvariant() switch
        {
            "alimentacao" or "restaurante" => taxaBase + 0.005m,
            "varejo" or "comercio"         => taxaBase + 0.003m,
            "servicos"                     => taxaBase + 0.007m,
            _                              => taxaBase
        };

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

internal record MensagemContratacao(int ContratacaoId);
