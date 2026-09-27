using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Entities.Financeiro;
using VisoERP.Infra.Data.Config.Cadastros;
using VisoERP.Infra.Data.Config.Comercial;
using VisoERP.Infra.Data.Config.Estoque;
using VisoERP.Infra.Data.Config.Financeiro;

namespace VisoERP.Infra.Data.Context;

public sealed class VisoErpDbContext(DbContextOptions<VisoErpDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Suprimento> Suprimentos => Set<Suprimento>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<PedidoFornecedor> PedidosFornecedores => Set<PedidoFornecedor>();
    public DbSet<ItemPedidoFornecedor> ItensPedidosFornecedores => Set<ItemPedidoFornecedor>();
    public DbSet<EntradaEstoque> EntradasEstoque => Set<EntradaEstoque>();
    public DbSet<ItemEntradaEstoque> ItensEntradasEstoque => Set<ItemEntradaEstoque>();
    public DbSet<EstoqueProduto> EstoquesProdutos => Set<EstoqueProduto>();
    public DbSet<LoteEstoque> LotesEstoque => Set<LoteEstoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVendas => Set<ItemVenda>();
    public DbSet<ParcelaVenda> ParcelasVendas => Set<ParcelaVenda>();
    public DbSet<RecebimentoVenda> RecebimentosVendas => Set<RecebimentoVenda>();
    public DbSet<Investimento> Investimentos => Set<Investimento>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<ContaPagar> ContasPagar => Set<ContaPagar>();
    public DbSet<PagamentoContaPagar> PagamentosContasPagar => Set<PagamentoContaPagar>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoriaConfig());
        modelBuilder.ApplyConfiguration(new SuprimentoConfig());
        modelBuilder.ApplyConfiguration(new ClienteConfig());
        modelBuilder.ApplyConfiguration(new FornecedorConfig());
        modelBuilder.ApplyConfiguration(new PedidoFornecedorConfig());
        modelBuilder.ApplyConfiguration(new ItemPedidoFornecedorConfig());
        modelBuilder.ApplyConfiguration(new EntradaEstoqueConfig());
        modelBuilder.ApplyConfiguration(new ItemEntradaEstoqueConfig());
        modelBuilder.ApplyConfiguration(new EstoqueProdutoConfig());
        modelBuilder.ApplyConfiguration(new LoteEstoqueConfig());
        modelBuilder.ApplyConfiguration(new MovimentacaoEstoqueConfig());
        modelBuilder.ApplyConfiguration(new VendaConfig());
        modelBuilder.ApplyConfiguration(new ItemVendaConfig());
        modelBuilder.ApplyConfiguration(new ParcelaVendaConfig());
        modelBuilder.ApplyConfiguration(new RecebimentoVendaConfig());
        modelBuilder.ApplyConfiguration(new InvestimentoConfig());
        modelBuilder.ApplyConfiguration(new DespesaConfig());
        modelBuilder.ApplyConfiguration(new ContaPagarConfig());
        modelBuilder.ApplyConfiguration(new PagamentoContaPagarConfig());
        base.OnModelCreating(modelBuilder);
    }
}
