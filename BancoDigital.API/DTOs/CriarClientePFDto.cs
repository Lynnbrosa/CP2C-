using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public class CriarClientePFDto
{
    [Required] public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    [Required] public int AgenciaId { get; set; }
    [Required] public string Cpf { get; set; } = string.Empty;
    [Required] public DateTime DataNascimento { get; set; }
    public decimal Renda { get; set; }
}
