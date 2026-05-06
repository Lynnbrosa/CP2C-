namespace BancoDigital.API.Entities;

public class ReceberSalario : Produto
{
    public string BancoConvenio { get; set; } = string.Empty;
    public decimal TaxaTransferencia { get; set; }
}
