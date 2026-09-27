using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Infra.Data.Config.Cadastros;

namespace VisoERP.Infra.Data.Context;

public sealed class VisoErpDbContext(DbContextOptions<VisoErpDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Suprimento> Suprimentos => Set<Suprimento>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoriaConfig());
        modelBuilder.ApplyConfiguration(new SuprimentoConfig());
        modelBuilder.ApplyConfiguration(new ClienteConfig());
        modelBuilder.ApplyConfiguration(new FornecedorConfig());
        base.OnModelCreating(modelBuilder);
    }
}
