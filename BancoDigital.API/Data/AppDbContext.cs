using BancoDigital.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoDigital.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<PessoaFisica> PessoasFisicas { get; set; }
    public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
    public DbSet<Agencia> Agencias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }
    public DbSet<MaquinaDeCartao> MaquinasDeCartao { get; set; }
    public DbSet<ReceberSalario> ReceberSalarios { get; set; }
    public DbSet<Contratacao> Contratacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasDiscriminator<string>("TipoCliente")
            .HasValue<PessoaFisica>("PF")
            .HasValue<PessoaJuridica>("PJ");

        modelBuilder.Entity<Produto>()
            .HasDiscriminator<string>("TipoProduto")
            .HasValue<Emprestimo>("EMPRESTIMO")
            .HasValue<MaquinaDeCartao>("MAQUINA_CARTAO")
            .HasValue<ReceberSalario>("RECEBER_SALARIO");

        modelBuilder.Entity<Cliente>()
            .HasOne(c => c.Agencia)
            .WithMany(a => a.Clientes)
            .HasForeignKey(c => c.AgenciaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contratacao>()
            .HasOne(c => c.Cliente)
            .WithMany(cl => cl.Contratacoes)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contratacao>()
            .HasOne(c => c.Produto)
            .WithMany()
            .HasForeignKey(c => c.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PessoaFisica>().Property(p => p.Renda).HasPrecision(18, 2);
        modelBuilder.Entity<Emprestimo>().Property(e => e.TaxaJuros).HasPrecision(5, 4);
        modelBuilder.Entity<Emprestimo>().Property(e => e.ValorMinimo).HasPrecision(18, 2);
        modelBuilder.Entity<Emprestimo>().Property(e => e.ValorMaximo).HasPrecision(18, 2);
        modelBuilder.Entity<MaquinaDeCartao>().Property(m => m.TaxaMdrBase).HasPrecision(5, 4);
        modelBuilder.Entity<ReceberSalario>().Property(r => r.TaxaTransferencia).HasPrecision(5, 4);
        modelBuilder.Entity<Contratacao>().Property(c => c.ValorSolicitado).HasPrecision(18, 2);
        modelBuilder.Entity<Contratacao>().Property(c => c.TaxaMdrAplicada).HasPrecision(5, 4);

        modelBuilder.Entity<Cliente>().ToTable("TB_CLIENTES");
        modelBuilder.Entity<Agencia>().ToTable("TB_AGENCIAS");
        modelBuilder.Entity<Produto>().ToTable("TB_PRODUTOS");
        modelBuilder.Entity<Contratacao>().ToTable("TB_CONTRATACOES");

        modelBuilder.Entity<Emprestimo>().HasData(new Emprestimo
        {
            Id = 1, Nome = "Emprestimo Pessoal", Descricao = "Emprestimo com analise de score",
            Ativo = true, TaxaJuros = 0.025m, ValorMinimo = 1000m, ValorMaximo = 50000m,
            PrazoMaximoMeses = 48, ScoreMinimo = 200
        });
        modelBuilder.Entity<MaquinaDeCartao>().HasData(new MaquinaDeCartao
        {
            Id = 2, Nome = "Maquina de Cartao Plus", Descricao = "Maquina WiFi com MDR variavel",
            Ativo = true, Modelo = "Model-X300", TipoConexao = "WiFi", TaxaMdrBase = 0.012m
        });
    }
}
