using BancoDigital.API.DTOs;
using BancoDigital.API.Entities;

namespace BancoDigital.API.Services;

public interface IContratacaoService
{
    Task<Contratacao> SolicitarAsync(CriarContratacaoDto dto);
    Task<Contratacao?> BuscarPorIdAsync(int id);
}
