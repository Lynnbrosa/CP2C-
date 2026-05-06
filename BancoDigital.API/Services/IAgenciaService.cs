using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;

namespace BancoDigital.API.Services;

public interface IAgenciaService
{
    Task<Agencia> CadastrarAsync(CriarAgenciaDto dto);
    Task<Agencia?> BuscarPorIdAsync(int id);
}
