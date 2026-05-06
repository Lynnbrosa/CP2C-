using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDigital.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_AGENCIAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Endereco = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_AGENCIAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRODUTOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoProduto = table.Column<string>(type: "NVARCHAR2(21)", maxLength: 21, nullable: false),
                    TaxaJuros = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: true),
                    ValorMinimo = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    ValorMaximo = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    PrazoMaximoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ScoreMinimo = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Modelo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TipoConexao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TaxaMdrBase = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: true),
                    BancoConvenio = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TaxaTransferencia = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRODUTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_CLIENTES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    AgenciaId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TipoCliente = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Renda = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    Cnpj = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: true),
                    RazaoSocial = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    RamoAtividade = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CLIENTES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CLIENTES_TB_AGENCIAS_AgenciaId",
                        column: x => x.AgenciaId,
                        principalTable: "TB_AGENCIAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONTRATACOES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ClienteId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProdutoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Observacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ValorSolicitado = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    PrazoMeses = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Score = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TaxaMdrAplicada = table.Column<decimal>(type: "DECIMAL(5,4)", precision: 5, scale: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONTRATACOES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACOES_TB_CLIENTES_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "TB_CLIENTES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_CONTRATACOES_TB_PRODUTOS_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TB_PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLIENTES_AgenciaId",
                table: "TB_CLIENTES",
                column: "AgenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACOES_ClienteId",
                table: "TB_CONTRATACOES",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONTRATACOES_ProdutoId",
                table: "TB_CONTRATACOES",
                column: "ProdutoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CONTRATACOES");

            migrationBuilder.DropTable(
                name: "TB_CLIENTES");

            migrationBuilder.DropTable(
                name: "TB_PRODUTOS");

            migrationBuilder.DropTable(
                name: "TB_AGENCIAS");
        }
    }
}
