using AutoMapper;
using VisoERP.Application.DTOs.Financeiro;
using VisoERP.Application.Interface.Financeiro;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Entities.Financeiro;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Estoque;

namespace VisoERP.Application.AppService.Financeiro;

public sealed class FinanceiroAppService(IRepository<Investimento> investimentos,
    IRepository<Despesa> despesas, IRepository<ContaPagar> contas,
    IRepository<PagamentoContaPagar> pagamentos, IRepository<RecebimentoVenda> recebimentos,
    IEstoqueRepository estoque, IVendaRepository vendas,
    IUnitOfWork unitOfWork, IMapper mapper) : IFinanceiroAppService
{
    public async Task<Guid> RegistrarInvestimentoAsync(string descricao, decimal valor,
        DateOnly data, CancellationToken ct)
    {
        var entidade = Investimento.Criar(descricao, valor, data);
        await investimentos.AdicionarAsync(entidade, ct);
        await unitOfWork.SalvarAlteracoesAsync(ct);
        return entidade.Id;
    }

    public async Task<Guid> RegistrarDespesaAsync(string descricao, decimal valor,
        DateOnly competencia, bool paga, CancellationToken ct)
    {
        var entidade = Despesa.Criar(descricao, valor, competencia);
        if (paga) entidade.MarcarPaga(DateTimeOffset.UtcNow);
        await despesas.AdicionarAsync(entidade, ct);
        await unitOfWork.SalvarAlteracoesAsync(ct);
        return entidade.Id;
    }

    public async Task PagarDespesaAsync(Guid id, CancellationToken ct)
    {
        var despesa = await despesas.ObterPorIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Despesa não encontrada.");
        despesa.MarcarPaga(DateTimeOffset.UtcNow);
        await unitOfWork.SalvarAlteracoesAsync(ct);
    }

    public async Task PagarContaAsync(Guid id, decimal valor, CancellationToken ct)
    {
        await unitOfWork.ExecutarEmTransacaoAsync(async token =>
        {
            var conta = await contas.ObterPorIdAsync(id, token)
                ?? throw new KeyNotFoundException("Conta não encontrada.");
            conta.RegistrarPagamento(valor);
            await pagamentos.AdicionarAsync(PagamentoContaPagar.Criar(id, valor,
                DateTimeOffset.UtcNow), token);
        }, ct);
    }

    public async Task<IReadOnlyList<InvestimentoDto>> ListarInvestimentosAsync(CancellationToken ct) =>
        mapper.Map<List<InvestimentoDto>>(await investimentos.ListarAsync(ct));

    public async Task<IReadOnlyList<DespesaDto>> ListarDespesasAsync(CancellationToken ct) =>
        mapper.Map<List<DespesaDto>>(await despesas.ListarAsync(ct));

    public async Task<IReadOnlyList<ContaPagarDto>> ListarContasPagarAsync(CancellationToken ct) =>
        mapper.Map<List<ContaPagarDto>>(await contas.ListarAsync(ct));

    public async Task<ResumoFinanceiroDto> ResumirAsync(DateOnly inicio, DateOnly fim, CancellationToken ct)
    {
        if (inicio == default || fim == default || inicio > fim)
            throw new ArgumentException("Período inválido.");
        var vendasLista = await vendas.ListarComItensEParcelasAsync(ct);
        var entradasLista = await estoque.ListarEntradasComItensAsync(ct);
        var despesasLista = await despesas.ListarAsync(ct);
        var investimentosLista = await investimentos.ListarAsync(ct);
        var contasLista = await contas.ListarAsync(ct);
        var recebimentosLista = await recebimentos.ListarAsync(ct);
        var pagamentosLista = await pagamentos.ListarAsync(ct);
        bool Periodo(DateOnly data) => data >= inicio && data <= fim;
        var vendasPeriodo = vendasLista.Where(x => x.Finalizada &&
            Periodo(DateOnly.FromDateTime(x.Data.Date))).ToList();
        var faturamento = vendasPeriodo.Sum(x => x.Total);
        var cpv = vendasPeriodo.Sum(x => x.CustoTotal);
        var despesasPeriodo = despesasLista.Where(x => Periodo(x.DataCompetencia)).Sum(x => x.Valor);
        var investimentosPeriodo = investimentosLista.Where(x => Periodo(x.Data)).Sum(x => x.Valor);
        var capitalInvestido = investimentosLista.Where(x => x.Data <= fim).Sum(x => x.Valor);
        var lucroBruto = faturamento - cpv;
        var lucroLiquido = lucroBruto - despesasPeriodo;
        var compras = entradasLista.Where(x => Periodo(DateOnly.FromDateTime(x.CriadaEm.Date)))
            .Sum(x => x.TotalCusto);
        var recebido = recebimentosLista.Where(x => Periodo(DateOnly.FromDateTime(x.Data.Date)))
            .Sum(x => x.Valor);
        var pagamentosCompras = pagamentosLista.Where(x => Periodo(DateOnly.FromDateTime(x.Data.Date)))
            .Sum(x => x.Valor);
        var vendasAteFim = vendasLista.Where(x => x.Finalizada &&
            DateOnly.FromDateTime(x.Data.Date) <= fim).ToList();
        var idsVendas = vendasAteFim.Select(x => x.Id).ToHashSet();
        var recebidoAteFim = recebimentosLista.Where(x => idsVendas.Contains(x.VendaId) &&
            DateOnly.FromDateTime(x.Data.Date) <= fim).Sum(x => x.Valor);
        var contasReceber = vendasAteFim.Sum(x => x.Total) - recebidoAteFim;
        var idsEntradas = entradasLista.Where(x => DateOnly.FromDateTime(x.CriadaEm.Date) <= fim)
            .Select(x => x.Id).ToHashSet();
        var contasAteFim = contasLista.Where(x => idsEntradas.Contains(x.EntradaEstoqueId)).ToList();
        var idsContas = contasAteFim.Select(x => x.Id).ToHashSet();
        var pagoAteFim = pagamentosLista.Where(x => idsContas.Contains(x.ContaPagarId) &&
            DateOnly.FromDateTime(x.Data.Date) <= fim).Sum(x => x.Valor);
        var contasPagar = contasAteFim.Sum(x => x.Valor) - pagoAteFim;
        return new ResumoFinanceiroDto(inicio, fim, faturamento, recebido, compras, cpv,
            lucroBruto, despesasPeriodo, lucroLiquido, investimentosPeriodo, capitalInvestido,
            contasReceber, contasPagar, pagamentosCompras,
            capitalInvestido > 0 ? Math.Round(lucroLiquido / capitalInvestido * 100, 2) : null);
    }
}
