namespace VisoERP.Domain.Entities.Financeiro;

public sealed class ContaPagar : EntidadeBase
{
    private ContaPagar() { }
    public Guid EntradaEstoqueId { get; private set; }
    public Guid? FornecedorId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public decimal ValorPago { get; private set; }
    public DateOnly Vencimento { get; private set; }
    public decimal Saldo => Valor - ValorPago;

    public static ContaPagar Criar(Guid entradaEstoqueId, Guid? fornecedorId,
        string descricao, decimal valor, DateOnly vencimento)
    {
        if (entradaEstoqueId == Guid.Empty) throw new ArgumentException("Entrada inválida.");
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Trim().Length > 200)
            throw new ArgumentException("Descrição inválida.");
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor));
        if (vencimento == default) throw new ArgumentException("Vencimento inválido.");
        return new ContaPagar { EntradaEstoqueId = entradaEstoqueId,
            FornecedorId = fornecedorId, Descricao = descricao.Trim(),
            Valor = valor, Vencimento = vencimento };
    }

    public void RegistrarPagamento(decimal valor)
    {
        if (valor <= 0 || valor > Saldo) throw new ArgumentOutOfRangeException(nameof(valor));
        ValorPago += valor;
    }
}
