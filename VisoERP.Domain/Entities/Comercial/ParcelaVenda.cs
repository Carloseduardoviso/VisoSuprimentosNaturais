namespace VisoERP.Domain.Entities.Comercial;

public sealed class ParcelaVenda : EntidadeBase
{
    private ParcelaVenda() { }
    public Guid VendaId { get; private set; }
    public int Numero { get; private set; }
    public DateOnly Vencimento { get; private set; }
    public decimal Valor { get; private set; }
    public decimal ValorPago { get; private set; }
    public DateTimeOffset? PagoEm { get; private set; }
    public decimal Saldo => Valor - ValorPago;

    internal static ParcelaVenda Criar(Guid vendaId, int numero, DateOnly vencimento, decimal valor)
    {
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor));
        return new ParcelaVenda { VendaId = vendaId, Numero = numero, Vencimento = vencimento, Valor = valor };
    }

    internal void RegistrarPagamento(decimal valor, DateTimeOffset data)
    {
        if (valor <= 0 || valor > Saldo) throw new ArgumentOutOfRangeException(nameof(valor));
        ValorPago += valor;
        if (Saldo == 0) PagoEm = data;
    }
}
