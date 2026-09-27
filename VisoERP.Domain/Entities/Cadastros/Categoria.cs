namespace VisoERP.Domain.Entities.Cadastros;

public sealed class Categoria : EntidadeBase
{
    private Categoria() { }

    public string Nome { get; private set; } = string.Empty;
    public bool Ativa { get; private set; } = true;

    public static Categoria Criar(string nome)
    {
        var categoria = new Categoria();
        categoria.Atualizar(nome, true);
        return categoria;
    }

    public void Atualizar(string nome, bool ativa)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length > 120)
            throw new ArgumentException("O nome da categoria deve ter de 1 a 120 caracteres.", nameof(nome));
        Nome = nome.Trim();
        Ativa = ativa;
    }
}
