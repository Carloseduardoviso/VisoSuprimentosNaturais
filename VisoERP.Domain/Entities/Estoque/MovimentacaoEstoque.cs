using VisoERP.Domain.Enums;

namespace VisoERP.Domain.Entities.Estoque;

public sealed class MovimentacaoEstoque : EntidadeBase
{
    private MovimentacaoEstoque() { }
    public Guid SuprimentoId { get; private set; }
    public Guid? LoteEstoqueId { get; private set; }
    public Guid ReferenciaId { get; private set; }
    public TipoMovimentacaoEstoque Tipo { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal CustoUnitario { get; private set; }
    public DateTimeOffset Data { get; private set; }

    public static MovimentacaoEstoque Criar(Guid suprimentoId, Guid? loteId, Guid referenciaId,
        TipoMovimentacaoEstoque tipo, decimal quantidade, decimal custoUnitario, DateTimeOffset data)
    {
        if (suprimentoId == Guid.Empty || referenciaId == Guid.Empty)
            throw new ArgumentException("Referência inválida.");
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        if (custoUnitario < 0) throw new ArgumentOutOfRangeException(nameof(custoUnitario));
        return new MovimentacaoEstoque
        {
            SuprimentoId = suprimentoId, LoteEstoqueId = loteId, ReferenciaId = referenciaId,
            Tipo = tipo, Quantidade = quantidade, CustoUnitario = custoUnitario, Data = data
        };
    }
}
