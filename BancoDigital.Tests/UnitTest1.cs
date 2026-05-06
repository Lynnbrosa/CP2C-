using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;
using BancoDigital.API.Messaging;
using BancoDigital.API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BancoDigital.Tests;

public class ClienteServiceTests
{
    private static AppDbContext CriarContexto() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<Agencia> AdicionarAgenciaAsync(AppDbContext ctx)
    {
        var ag = new Agencia { Nome = "Agência Central", Codigo = "0001" };
        ctx.Agencias.Add(ag);
        await ctx.SaveChangesAsync();
        return ag;
    }

    [Fact]
    public async Task CadastrarPF_Sucesso_DeveRetornarClienteCriado()
    {
        using var ctx = CriarContexto();
        var ag = await AdicionarAgenciaAsync(ctx);
        var svc = new ClienteService(ctx);

        var resultado = await svc.CadastrarPFAsync(new CriarClientePFDto
        {
            Nome = "João Silva", Email = "joao@email.com",
            AgenciaId = ag.Id, Cpf = "123.456.789-00",
            DataNascimento = new DateTime(1990, 1, 1), Renda = 3000
        });

        Assert.NotNull(resultado);
        Assert.Equal("12345678900", ((PessoaFisica)resultado).Cpf);
    }

    [Fact]
    public async Task CadastrarPF_CpfDuplicado_DeveLancarExcecao()
    {
        using var ctx = CriarContexto();
        var ag = await AdicionarAgenciaAsync(ctx);
        var svc = new ClienteService(ctx);
        var dto = new CriarClientePFDto
        {
            Nome = "João", Email = "joao@email.com", AgenciaId = ag.Id,
            Cpf = "123.456.789-00", DataNascimento = new DateTime(1990, 1, 1), Renda = 3000
        };

        await svc.CadastrarPFAsync(dto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CadastrarPFAsync(dto));
    }

    [Fact]
    public async Task CadastrarPJ_Sucesso_DeveRetornarClienteCriado()
    {
        using var ctx = CriarContexto();
        var ag = await AdicionarAgenciaAsync(ctx);
        var svc = new ClienteService(ctx);

        var resultado = await svc.CadastrarPJAsync(new CriarClientePJDto
        {
            Nome = "Empresa XYZ", Email = "contato@empresa.com",
            AgenciaId = ag.Id, Cnpj = "12.345.678/0001-90",
            RazaoSocial = "Empresa XYZ Ltda", RamoAtividade = "varejo"
        });

        Assert.NotNull(resultado);
        Assert.Equal("12345678000190", ((PessoaJuridica)resultado).Cnpj);
    }

    [Fact]
    public async Task CadastrarPJ_CnpjDuplicado_DeveLancarExcecao()
    {
        using var ctx = CriarContexto();
        var ag = await AdicionarAgenciaAsync(ctx);
        var svc = new ClienteService(ctx);
        var dto = new CriarClientePJDto
        {
            Nome = "Empresa", Email = "emp@email.com", AgenciaId = ag.Id,
            Cnpj = "12.345.678/0001-90", RazaoSocial = "Empresa Ltda"
        };

        await svc.CadastrarPJAsync(dto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CadastrarPJAsync(dto));
    }

    [Fact]
    public async Task CadastrarPF_AgenciaInexistente_DeveLancarExcecao()
    {
        using var ctx = CriarContexto();
        var svc = new ClienteService(ctx);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            svc.CadastrarPFAsync(new CriarClientePFDto
            {
                Nome = "Teste", Email = "t@t.com", AgenciaId = 999,
                Cpf = "111.111.111-11", DataNascimento = new DateTime(1990, 1, 1), Renda = 1000
            }));
    }
}

public class ContratacaoServiceTests
{
    private static AppDbContext CriarContexto() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<(AppDbContext, int clienteId, int produtoId)> PrepararAsync()
    {
        var ctx = CriarContexto();
        var ag = new Agencia { Nome = "Agência Test", Codigo = "0001" };
        ctx.Agencias.Add(ag);
        await ctx.SaveChangesAsync();

        var pf = new PessoaFisica
        {
            Nome = "Teste", Email = "t@t.com", AgenciaId = ag.Id,
            Cpf = "11111111111", DataNascimento = new DateTime(1990, 1, 1), Renda = 5000
        };
        ctx.PessoasFisicas.Add(pf);

        var emp = new Emprestimo
        {
            Nome = "Empréstimo Pessoal", TaxaJuros = 2.5m,
            ValorMinimo = 1000, ValorMaximo = 50000,
            PrazoMaximoMeses = 48, ScoreMinimo = 100, Ativo = true
        };
        ctx.Emprestimos.Add(emp);
        await ctx.SaveChangesAsync();

        return (ctx, pf.Id, emp.Id);
    }

    [Fact]
    public async Task SolicitarContratacao_Valida_DevePublicarNaFilaERetornarPendente()
    {
        var (ctx, clienteId, produtoId) = await PrepararAsync();
        var publisherMock = new Mock<IRabbitMQPublisher>();
        var svc = new ContratacaoService(ctx, publisherMock.Object);

        var resultado = await svc.SolicitarAsync(new CriarContratacaoDto
        {
            ClienteId = clienteId, ProdutoId = produtoId,
            ValorSolicitado = 5000, PrazoMeses = 12
        });

        Assert.Equal(StatusContratacao.Pendente, resultado.Status);
        publisherMock.Verify(p => p.Publicar(resultado.Id), Times.Once);
    }

    [Fact]
    public async Task SolicitarContratacao_ClienteInexistente_DeveLancarExcecao()
    {
        var (ctx, _, produtoId) = await PrepararAsync();
        var publisherMock = new Mock<IRabbitMQPublisher>();
        var svc = new ContratacaoService(ctx, publisherMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            svc.SolicitarAsync(new CriarContratacaoDto
            {
                ClienteId = 9999, ProdutoId = produtoId,
                ValorSolicitado = 5000, PrazoMeses = 12
            }));
    }

    [Fact]
    public async Task BuscarContratacao_DeveRetornarStatusAtualizado()
    {
        var (ctx, clienteId, produtoId) = await PrepararAsync();
        var publisherMock = new Mock<IRabbitMQPublisher>();
        var svc = new ContratacaoService(ctx, publisherMock.Object);

        var contratacao = await svc.SolicitarAsync(new CriarContratacaoDto
        {
            ClienteId = clienteId, ProdutoId = produtoId,
            ValorSolicitado = 5000, PrazoMeses = 12
        });

        var resultado = await svc.BuscarPorIdAsync(contratacao.Id);

        Assert.NotNull(resultado);
        Assert.Equal(contratacao.Id, resultado!.Id);
        Assert.Equal(StatusContratacao.Pendente, resultado.Status);
    }

    [Fact]
    public async Task VincularCliente_AgenciaInexistente_DeveLancarExcecao()
    {
        using var ctx = CriarContexto();
        var svc = new ClienteService(ctx);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            svc.CadastrarPFAsync(new CriarClientePFDto
            {
                Nome = "Test", Email = "t@t.com", AgenciaId = 999,
                Cpf = "000.000.000-00", DataNascimento = DateTime.Today.AddYears(-30), Renda = 1000
            }));
    }
}