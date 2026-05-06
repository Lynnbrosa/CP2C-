using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.DTOs;

public class CriarContratacaoDto
{
    [Required] public int ClienteId { get; set; }
    [Required] public int ProdutoId { get; set; }
    public decimal? ValorSolicitado { get; set; }
    public int? PrazoMeses { get; set; }
}
