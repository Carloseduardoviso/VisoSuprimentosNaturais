namespace VisoERP.Domain.Entities.Estoque;

public sealed class LoteEstoque : EntidadeBase
{
    private LoteEstoque() { }
    public Guid SuprimentoId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public DateOnly? Validade { get; private set; }
    public decimal Quantidade { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public static LoteEstoque Criar(Guid suprimentoId, string codigo, DateOnly? validade)
    {
        if (suprimentoId == Guid.Empty) throw new ArgumentException("Suprimento inválido.", nameof(suprimentoId));
        if (string.IsNullOrWhiteSpace(codigo) || codigo.Trim().Length > 80)
            throw new ArgumentException("Código do lote inválido.", nameof(codigo));
        return new LoteEstoque { SuprimentoId = suprimentoId, Codigo = codigo.Trim(), Validade = validade };
    }

    public void RegistrarEntrada(decimal quantidade)
    {
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        Quantidade += quantidade;
    }

    public void RegistrarSaida(decimal quantidade)
    {
        if (quantidade <= 0 || quantidade > Quantidade)
            throw new InvalidOperationException("Quantidade indisponível no lote.");
        Quantidade -= quantidade;
    }
}
