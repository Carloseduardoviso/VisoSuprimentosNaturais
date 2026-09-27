namespace VisoERP.Application.DTOs.Financeiro;

public sealed record InvestimentoDto(Guid Id, string Descricao, decimal Valor, DateOnly Data);
public sealed record DespesaDto(Guid Id, string Descricao, decimal Valor,
    DateOnly DataCompetencia, DateTimeOffset? PagaEm);
public sealed record ContaPagarDto(Guid Id, Guid EntradaEstoqueId, Guid? FornecedorId,
    string Descricao, decimal Valor, decimal ValorPago, decimal Saldo, DateOnly Vencimento);

public sealed record ResumoFinanceiroDto(DateOnly Inicio, DateOnly Fim,
    decimal Faturamento, decimal Recebido, decimal Compras, decimal CustoProdutosVendidos,
    decimal LucroBruto, decimal Despesas, decimal LucroLiquido,
    decimal InvestimentoPeriodo, decimal CapitalInvestidoAteFim,
    decimal ContasReceber, decimal ContasPagar, decimal PagamentosCompras,
    decimal? RetornoSobreInvestimentoPercentual);
