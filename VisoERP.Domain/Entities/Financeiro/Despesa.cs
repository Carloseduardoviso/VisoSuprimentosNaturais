namespace VisoERP.Domain.Entities.Financeiro;

public sealed class Despesa : EntidadeBase
{
    private Despesa() { }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateOnly DataCompetencia { get; private set; }
    public DateTimeOffset? PagaEm { get; private set; }

    public static Despesa Criar(string descricao, decimal valor, DateOnly dataCompetencia)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Trim().Length > 200)
            throw new ArgumentException("Descri��o inv�lida.");
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor));
        if (dataCompetencia == default) throw new ArgumentException("Data inv�lida.");
        return new Despesa { Descricao = descricao.Trim(), Valor = valor,
            DataCompetencia = dataCompetencia };
    }

    public void MarcarPaga(DateTimeOffset data)
    {
        if (PagaEm is not null) throw new InvalidOperationException("Despesa j� paga.");
        if (data == default) throw new ArgumentException("Data inv�lida.");
        PagaEm = data;
    }
}
