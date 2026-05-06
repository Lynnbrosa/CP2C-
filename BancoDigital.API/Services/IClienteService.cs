using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;

namespace BancoDigital.API.Services;

public interface IClienteService
{
    Task<Cliente> CadastrarPFAsync(CriarClientePFDto dto);
    Task<Cliente> CadastrarPJAsync(CriarClientePJDto dto);
    Task<Cliente?> BuscarPorIdAsync(int id);
}
