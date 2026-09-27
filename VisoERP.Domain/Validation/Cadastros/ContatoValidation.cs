namespace VisoERP.Domain.Validation.Cadastros;

internal static class ContatoValidation
{
    public static (string Nome, string? Documento, string? Email, string? Telefone) Validar(
        string nome, string? documento, string? email, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length > 200)
            throw new ArgumentException("Nome deve ter de 1 a 200 caracteres.", nameof(nome));
        documento = Limpar(documento);
        email = Limpar(email);
        telefone = Limpar(telefone);
        if (documento?.Length > 20) throw new ArgumentException("Documento muito longo.", nameof(documento));
        if (email?.Length > 256 || (email is not null && !System.Net.Mail.MailAddress.TryCreate(email, out _)))
            throw new ArgumentException("E-mail inválido.", nameof(email));
        if (telefone?.Length > 30) throw new ArgumentException("Telefone muito longo.", nameof(telefone));
        return (nome.Trim(), documento, email, telefone);
    }

    private static string? Limpar(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
