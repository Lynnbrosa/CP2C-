using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Services;

public class AgenciaService : IAgenciaService
{
    private readonly AppDbContext _context;

    public AgenciaService(AppDbContext context) => _context = context;

    public async Task<Agencia> CadastrarAsync(CriarAgenciaDto dto)
    {
        var agencia = new Agencia { Nome = dto.Nome, Codigo = dto.Codigo, Endereco = dto.Endereco };
        _context.Agencias.Add(agencia);
        await _context.SaveChangesAsync();
        return agencia;
    }

    public async Task<Agencia?> BuscarPorIdAsync(int id)
        => await _context.Agencias.Include(a => a.Clientes).FirstOrDefaultAsync(a => a.Id == id);
}
