using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public class CriarClientePJDto
{
    [Required] public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    [Required] public int AgenciaId { get; set; }
    [Required] public string Cnpj { get; set; } = string.Empty;
    [Required] public string RazaoSocial { get; set; } = string.Empty;
    public string? RamoAtividade { get; set; }
}
