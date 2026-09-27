namespace VisoERP.Domain.Entities;

public sealed class Usuario
{
    public static string NormalizarCpf(string cpf) => new(cpf.Where(char.IsDigit).ToArray());

    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
}
