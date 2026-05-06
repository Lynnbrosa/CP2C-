using System.ComponentModel.DataAnnotations;

namespace BancoDigital.API.Entities;

public abstract class Cliente
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; set; }

    public int AgenciaId { get; set; }
    public Agencia Agencia { get; set; } = null!;

    public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();
}
