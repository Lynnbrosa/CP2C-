using BancoDigital.API.DTOs;
using BancoDigital.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoDigital.API.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service) => _service = service;

    /// <summary>Cadastra pessoa física</summary>
    [HttpPost("pf")]
    public async Task<IActionResult> CadastrarPF([FromBody] CriarClientePFDto dto)
    {
        try
        {
            var cliente = await _service.CadastrarPFAsync(dto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = cliente.Id }, cliente);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { erro = ex.Message }); }
    }

    /// <summary>Cadastra pessoa jurídica</summary>
    [HttpPost("pj")]
    public async Task<IActionResult> CadastrarPJ([FromBody] CriarClientePJDto dto)
    {
        try
        {
            var cliente = await _service.CadastrarPJAsync(dto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = cliente.Id }, cliente);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { erro = ex.Message }); }
    }

    /// <summary>Busca cliente por ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var cliente = await _service.BuscarPorIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }
}
