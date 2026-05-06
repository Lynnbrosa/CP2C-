namespace BancoDigital.API.Entities;

public class MaquinaDeCartao : Produto
{
    public string Modelo { get; set; } = string.Empty;
    public string TipoConexao { get; set; } = string.Empty;
    public decimal TaxaMdrBase { get; set; }
}
