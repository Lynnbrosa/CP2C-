using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;
using BancoDigital.API.Messaging;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Services;

public class ContratacaoService : IContratacaoService
{
    private readonly AppDbContext _context;
    private readonly IRabbitMQPublisher _publisher;

    public ContratacaoService(AppDbContext context, IRabbitMQPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Contratacao> SolicitarAsync(CriarContratacaoDto dto)
    {
        _ = await _context.Clientes.FindAsync(dto.ClienteId)
            ?? throw new KeyNotFoundException("Cliente não encontrado.");

        _ = await _context.Produtos.FindAsync(dto.ProdutoId)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        var contratacao = new Contratacao
        {
            ClienteId = dto.ClienteId,
            ProdutoId = dto.ProdutoId,
            ValorSolicitado = dto.ValorSolicitado,
            PrazoMeses = dto.PrazoMeses,
            Status = StatusContratacao.Pendente,
            DataSolicitacao = DateTime.UtcNow
        };

        _context.Contratacoes.Add(contratacao);
        await _context.SaveChangesAsync();

        _publisher.Publicar(contratacao.Id);

        return contratacao;
    }

    public async Task<Contratacao?> BuscarPorIdAsync(int id)
        => await _context.Contratacoes
            .Include(c => c.Cliente)
            .Include(c => c.Produto)
            .FirstOrDefaultAsync(c => c.Id == id);
}
