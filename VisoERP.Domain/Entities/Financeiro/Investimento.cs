namespace VisoERP.Domain.Entities.Financeiro;

public sealed class Investimento : EntidadeBase
{
    private Investimento() { }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateOnly Data { get; private set; }

    public static Investimento Criar(string descricao, decimal valor, DateOnly data)
    {
        if (string.IsNullOrWhiteSpace(descricao) || descricao.Trim().Length > 200)
            throw new ArgumentException("Descrição inválida.");
        if (valor <= 0) throw new ArgumentOutOfRangeException(nameof(valor));
        if (data == default) throw new ArgumentException("Data inválida.");
        return new Investimento { Descricao = descricao.Trim(), Valor = valor, Data = data };
    }
}
