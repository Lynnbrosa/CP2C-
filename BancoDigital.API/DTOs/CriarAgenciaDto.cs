using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public class CriarAgenciaDto
{
    [Required] public string Nome { get; set; } = string.Empty;
    [Required] public string Codigo { get; set; } = string.Empty;
    public string? Endereco { get; set; }
}
