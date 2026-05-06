using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDigital.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TB_PRODUTOS",
                columns: new[] { "Id", "Ativo", "Descricao", "Nome", "PrazoMaximoMeses", "ScoreMinimo", "TaxaJuros", "TipoProduto", "ValorMaximo", "ValorMinimo" },
                values: new object[] { 1, true, "Emprestimo com analise de score", "Emprestimo Pessoal", 48, 200, 0.025m, "EMPRESTIMO", 50000m, 1000m });

            migrationBuilder.InsertData(
                table: "TB_PRODUTOS",
                columns: new[] { "Id", "Ativo", "Descricao", "Modelo", "Nome", "TaxaMdrBase", "TipoConexao", "TipoProduto" },
                values: new object[] { 2, true, "Maquina WiFi com MDR variavel", "Model-X300", "Maquina de Cartao Plus", 0.012m, "WiFi", "MAQUINA_CARTAO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TB_PRODUTOS",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TB_PRODUTOS",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
