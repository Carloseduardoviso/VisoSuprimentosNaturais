using VisoERP.Domain.Validation.Cadastros;

namespace VisoERP.Domain.Entities.Cadastros;

public sealed class Cliente : EntidadeBase
{
    private Cliente() { }
    public string Nome { get; private set; } = string.Empty;
    public string? Documento { get; private set; }
    public string? Email { get; private set; }
    public string? Telefone { get; private set; }
    public string? Cep { get; private set; }
    public string? Numero { get; private set; }
    public string? Endereco { get; private set; }
    public bool Ativo { get; private set; } = true;

    public static Cliente Criar(string nome, string? documento, string? email, string? telefone)
    {
        var cliente = new Cliente();
        cliente.Atualizar(nome, documento, email, telefone, true);
        return cliente;
    }

    public void Atualizar(string nome, string? documento, string? email, string? telefone, bool ativo, string? cep = null, string? numero = null, string? endereco = null)
    {
        var dados = ContatoValidation.Validar(nome, documento, email, telefone);
        Nome = dados.Nome;
        Documento = dados.Documento;
        Email = dados.Email;
        Telefone = dados.Telefone;
        Cep = cep?.Trim(); Numero = numero?.Trim(); Endereco = endereco?.Trim();
        Ativo = ativo;
    }
}
