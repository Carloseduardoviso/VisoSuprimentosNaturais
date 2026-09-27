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
            throw new ArgumentException("Descrição inválida.");
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor));
        if (dataCompetencia == default) throw new ArgumentException("Data inválida.");
        return new Despesa { Descricao = descricao.Trim(), Valor = valor,
            DataCompetencia = dataCompetencia };
    }

    public void MarcarPaga(DateTimeOffset data)
    {
        if (PagaEm is not null) throw new InvalidOperationException("Despesa já paga.");
        if (data == default) throw new ArgumentException("Data inválida.");
        PagaEm = data;
    }
}
