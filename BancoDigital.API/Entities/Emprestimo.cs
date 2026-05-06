namespace BancoDigital.API.Entities;

public class Emprestimo : Produto
{
    public decimal TaxaJuros { get; set; }
    public decimal ValorMinimo { get; set; }
    public decimal ValorMaximo { get; set; }
    public int PrazoMaximoMeses { get; set; }
    public int ScoreMinimo { get; set; }
}
