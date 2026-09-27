namespace VisoERP.Domain.Entities.Comercial;

public sealed class RecebimentoVenda : EntidadeBase
{
    private RecebimentoVenda() { }
    public Guid VendaId { get; private set; }
    public Guid? ParcelaVendaId { get; private set; }
    public decimal Valor { get; private set; }
    public DateTimeOffset Data { get; private set; }

    public static RecebimentoVenda Criar(Guid vendaId, Guid? parcelaVendaId, decimal valor, DateTimeOffset data)
    {
        if (vendaId == Guid.Empty || valor <= 0 || data == default)
            throw new ArgumentException("Recebimento inválido.");
        return new RecebimentoVenda { VendaId = vendaId, ParcelaVendaId = parcelaVendaId,
            Valor = valor, Data = data };
    }
}
