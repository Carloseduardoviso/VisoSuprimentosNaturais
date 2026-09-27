using VisoERP.Domain.Validation.Cadastros;

namespace VisoERP.Domain.Entities.Cadastros;

public sealed class Fornecedor : EntidadeBase
{
    private Fornecedor() { }
    public string Nome { get; private set; } = string.Empty;
    public string? Documento { get; private set; }
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public bool Ativo { get; private set; } = true;

    public static Fornecedor Criar(string nome, string? documento, string? email, string? telefone)
    {
        var fornecedor = new Fornecedor();
        fornecedor.Atualizar(nome, documento, email, telefone, true);
        return fornecedor;
    }

    public void Atualizar(string nome, string? documento, string? email, string? telefone, bool ativo)
    {
        var dados = ContatoValidation.Validar(nome, documento, email, telefone);
        Nome = dados.Nome;
        Documento = dados.Documento;
        Email = dados.Email;
        Telefone = dados.Telefone;
        Ativo = ativo;
    }
}
