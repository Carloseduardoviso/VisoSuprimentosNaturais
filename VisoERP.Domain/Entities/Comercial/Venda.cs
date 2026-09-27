namespace VisoERP.Domain.Entities.Comercial;

public sealed class Venda : EntidadeBase
{
    private readonly List<ItemVenda> _itens = [];
    private readonly List<ParcelaVenda> _parcelas = [];
    private readonly List<RecebimentoVenda> _recebimentos = [];
    private Venda() { }
    public Guid ClienteId { get; private set; }
    public DateTimeOffset Data { get; private set; } = DateTimeOffset.UtcNow;
    public decimal Desconto { get; private set; }
    public decimal ValorEntrada { get; private set; }
    public int NumeroParcelas { get; private set; }
    public DateOnly PrimeiroVencimento { get; private set; }
    public bool Finalizada { get; private set; }
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();
    public IReadOnlyCollection<ParcelaVenda> Parcelas => _parcelas.AsReadOnly();
    public IReadOnlyCollection<RecebimentoVenda> Recebimentos => _recebimentos.AsReadOnly();
    public decimal Subtotal => _itens.Sum(x => x.Total);
    public decimal Total => Subtotal - Desconto;
    public decimal CustoTotal => _itens.Sum(x => x.CustoTotal);
    public decimal ValorRecebido => ValorEntrada + _parcelas.Sum(x => x.ValorPago);

    public static Venda Criar(Guid clienteId, decimal desconto, decimal valorEntrada,
        int numeroParcelas, DateOnly primeiroVencimento)
    {
        if (clienteId == Guid.Empty) throw new ArgumentException("Cliente inválido.");
        if (desconto < 0 || valorEntrada < 0) throw new ArgumentOutOfRangeException(nameof(desconto));
        if (numeroParcelas is < 0 or > 12) throw new ArgumentOutOfRangeException(nameof(numeroParcelas));
        return new Venda { ClienteId = clienteId, Desconto = desconto, ValorEntrada = valorEntrada,
            NumeroParcelas = numeroParcelas, PrimeiroVencimento = primeiroVencimento };
    }

    public void AdicionarItem(Guid suprimentoId, decimal quantidade, decimal precoCatalogo,
        decimal precoUnitario, bool promocional, decimal custoUnitario, DateTimeOffset? data = null)
    {
        if (Finalizada) throw new InvalidOperationException("Venda já finalizada.");
        if (_itens.Any(x => x.SuprimentoId == suprimentoId))
            throw new InvalidOperationException("Suplemento alimentar duplicado na venda.");
        _itens.Add(ItemVenda.Criar(Id, suprimentoId, quantidade, precoCatalogo,
            precoUnitario, promocional, custoUnitario, data ?? DateTimeOffset.UtcNow));
    }

    public void Finalizar()
    {
        if (Finalizada) throw new InvalidOperationException("Venda já finalizada.");
        if (_itens.Count == 0) throw new InvalidOperationException("Venda sem suplementos alimentares.");
        if (Desconto > Subtotal) throw new InvalidOperationException("Desconto supera o subtotal.");
        if (ValorEntrada > Total) throw new InvalidOperationException("Entrada financeira supera o total.");
        var saldo = Total - ValorEntrada;
        if (saldo == 0 && NumeroParcelas != 0 || saldo > 0 && NumeroParcelas == 0)
            throw new InvalidOperationException("Número de parcelas incompatível com o saldo.");
        if (saldo == 0) { Finalizada = true; return; }
        if (PrimeiroVencimento == default) throw new InvalidOperationException("Informe o primeiro vencimento.");
        var centavos = decimal.Round(saldo * 100, 0, MidpointRounding.AwayFromZero);
        if (centavos != saldo * 100 || centavos < NumeroParcelas)
            throw new InvalidOperationException("Saldo insuficiente para o número de parcelas.");
        var valorBase = Math.Floor(centavos / NumeroParcelas);
        var extras = centavos - valorBase * NumeroParcelas;
        for (var numero = 1; numero <= NumeroParcelas; numero++)
            _parcelas.Add(ParcelaVenda.Criar(Id, numero, PrimeiroVencimento.AddMonths(numero - 1),
                (valorBase + (numero <= extras ? 1 : 0)) / 100));
        Finalizada = true;
    }

    public void RegistrarPagamento(Guid parcelaId, decimal valor, DateTimeOffset data)
    {
        var parcela = _parcelas.SingleOrDefault(x => x.Id == parcelaId)
            ?? throw new KeyNotFoundException("Parcela não encontrada.");
        parcela.RegistrarPagamento(valor, data);
    }
}
