using BancoDigital.API.Data;
using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;

    public ClienteService(AppDbContext context) => _context = context;

    public async Task<Cliente> CadastrarPFAsync(CriarClientePFDto dto)
    {
        _ = await _context.Agencias.FindAsync(dto.AgenciaId)
            ?? throw new KeyNotFoundException("Agência não encontrada.");

        var cpf = dto.Cpf.Replace(".", "").Replace("-", "");
        if (await _context.PessoasFisicas.AnyAsync(p => p.Cpf == cpf))
            throw new InvalidOperationException("CPF já cadastrado.");

        var pf = new PessoaFisica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cpf = cpf,
            DataNascimento = dto.DataNascimento,
            Renda = dto.Renda
        };

        _context.PessoasFisicas.Add(pf);
        await _context.SaveChangesAsync();
        return pf;
    }

    public async Task<Cliente> CadastrarPJAsync(CriarClientePJDto dto)
    {
        _ = await _context.Agencias.FindAsync(dto.AgenciaId)
            ?? throw new KeyNotFoundException("Agência não encontrada.");

        var cnpj = dto.Cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
        if (await _context.PessoasJuridicas.AnyAsync(p => p.Cnpj == cnpj))
            throw new InvalidOperationException("CNPJ já cadastrado.");

        var pj = new PessoaJuridica
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            AgenciaId = dto.AgenciaId,
            Cnpj = cnpj,
            RazaoSocial = dto.RazaoSocial,
            RamoAtividade = dto.RamoAtividade
        };

        _context.PessoasJuridicas.Add(pj);
        await _context.SaveChangesAsync();
        return pj;
    }

    public async Task<Cliente?> BuscarPorIdAsync(int id)
        => await _context.Clientes.Include(c => c.Agencia).FirstOrDefaultAsync(c => c.Id == id);
}
