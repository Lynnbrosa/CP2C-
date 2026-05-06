using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.Entities;

public class Agencia
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Endereco { get; set; }

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
