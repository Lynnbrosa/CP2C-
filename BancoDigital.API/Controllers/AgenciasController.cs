using BancoDigital.API.DTOs;
using BancoDigital.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/agencias")]
public class AgenciasController : ControllerBase
{
    private readonly IAgenciaService _service;

    public AgenciasController(IAgenciaService service) => _service = service;

    /// <summary>Cadastra agência</summary>
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CriarAgenciaDto dto)
    {
        var agencia = await _service.CadastrarAsync(dto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = agencia.Id }, agencia);
    }

    /// <summary>Busca agência por ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var agencia = await _service.BuscarPorIdAsync(id);
        return agencia is null ? NotFound() : Ok(agencia);
    }
}
