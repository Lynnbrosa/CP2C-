using BancoDigital.API.DTOs;
using BancoDigital.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/contratacoes")]
public class ContratacoesController : ControllerBase
{
    private readonly IContratacaoService _service;

    public ContratacoesController(IContratacaoService service) => _service = service;

    /// <summary>Solicita contratação — publica na fila RabbitMQ</summary>
    [HttpPost]
    public async Task<IActionResult> Solicitar([FromBody] CriarContratacaoDto dto)
    {
        try
        {
            var contratacao = await _service.SolicitarAsync(dto);
            return AcceptedAtAction(nameof(BuscarPorId), new { id = contratacao.Id }, contratacao);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
    }

    /// <summary>Consulta status da contratação</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var contratacao = await _service.BuscarPorIdAsync(id);
        return contratacao is null ? NotFound() : Ok(contratacao);
    }
}
