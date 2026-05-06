using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.Entities;

public class PessoaJuridica : Cliente
{
    [Required, MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string RazaoSocial { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? RamoAtividade { get; set; }
}
