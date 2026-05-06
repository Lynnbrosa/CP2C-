using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.Entities;

public class PessoaFisica : Cliente
{
    [Required, MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

    public DateTime DataNascimento { get; set; }

    public decimal Renda { get; set; }
}
