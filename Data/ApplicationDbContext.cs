using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Perfil> Perfis => Set<Perfil>();
    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();
    public DbSet<UnidadeFranqueada> Unidades => Set<UnidadeFranqueada>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<ProdutoServico> Produtos => Set<ProdutoServico>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque =>
        Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Royalty> Royalties => Set<Royalty>();
    public DbSet<ChamadoSuporte> Chamados => Set<ChamadoSuporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UnidadeFranqueada>()
            .HasIndex(u => u.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Franqueadora>()
            .HasIndex(f => f.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Fornecedor>()
            .HasIndex(f => f.Cnpj)
            .IsUnique();

        modelBuilder.Entity<Perfil>()
            .HasMany(p => p.Usuarios)
            .WithOne(u => u.Perfil)
            .HasForeignKey(u => u.PerfilId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Franqueadora>()
            .HasMany(f => f.Unidades)
            .WithOne(u => u.Franqueadora)
            .HasForeignKey(u => u.FranqueadoraId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Categoria>()
            .HasMany(c => c.Produtos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UnidadeFranqueada>()
            .HasMany(u => u.Vendas)
            .WithOne(v => v.UnidadeFranqueada)
            .HasForeignKey(v => v.UnidadeFranqueadaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venda>()
            .HasMany(v => v.Itens)
            .WithOne(i => i.Venda)
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProdutoServico>()
            .HasMany(p => p.ItensVenda)
            .WithOne(i => i.ProdutoServico)
            .HasForeignKey(i => i.ProdutoServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Estoque>()
            .HasIndex(e => new
            {
                e.UnidadeFranqueadaId,
                e.ProdutoServicoId
            })
            .IsUnique();

        modelBuilder.Entity<ProdutoServico>()
            .Property(p => p.PrecoBase)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .Property(v => v.ValorTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ItemVenda>()
            .Property(i => i.PrecoUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ItemVenda>()
            .Property(i => i.Subtotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Royalty>()
            .Property(r => r.Percentual)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Royalty>()
            .Property(r => r.FaturamentoPeriodo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Royalty>()
            .Property(r => r.ValorRoyalty)
            .HasPrecision(18, 2);
    }
}