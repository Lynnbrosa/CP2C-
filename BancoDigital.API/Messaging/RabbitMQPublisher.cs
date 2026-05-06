using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BancoDigital.API.Messaging;

public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMQPublisher> _logger;
    private readonly IConfiguration _configuration;
    private const string QueueName = "contratacoes";

    public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
        TryConnect();
    }

    private void TryConnect()
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
            _logger.LogInformation("RabbitMQ conectado.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("RabbitMQ indisponível: {msg}. Publicação desativada.", ex.Message);
        }
    }

    public void Publicar(int contratacaoId)
    {
        if (_channel is null || !_channel.IsOpen)
        {
            _logger.LogWarning("Canal RabbitMQ não disponível. Contratação {Id} não publicada.", contratacaoId);
            return;
        }
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { ContratacaoId = contratacaoId }));
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: props, body: body);
        _logger.LogInformation("Contratação {Id} publicada na fila.", contratacaoId);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
