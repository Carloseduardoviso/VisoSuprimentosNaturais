namespace VisoERP.Domain.Entities.Estoque;

public sealed class EstoqueProduto : EntidadeBase
{
    private EstoqueProduto() { }

    public Guid SuprimentoId { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal CustoMedio { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public static EstoqueProduto Criar(Guid suprimentoId)
    {
        if (suprimentoId == Guid.Empty) throw new ArgumentException("Suplemento alimentar inválido.", nameof(suprimentoId));
        return new EstoqueProduto { SuprimentoId = suprimentoId };
    }

    public void RegistrarEntrada(decimal quantidade, decimal custoUnitario)
    {
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (custoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(custoUnitario));
        CustoMedio = Math.Round((Quantidade * CustoMedio + quantidade * custoUnitario) /
            (Quantidade + quantidade), 4, MidpointRounding.AwayFromZero);
        Quantidade += quantidade;
    }

    public decimal RegistrarSaida(decimal quantidade)
    {
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (quantidade > Quantidade) throw new InvalidOperationException("Estoque insuficiente.");
        Quantidade -= quantidade;
        return CustoMedio;
    }
}
