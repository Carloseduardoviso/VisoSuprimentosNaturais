using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Cadastros;

public sealed class ClienteViewModel
{
    public Guid? Id { get; set; }
    [Required, StringLength(200)]
    public string Nome { get; set; } = string.Empty;
    [StringLength(14)] public string? Cpf { get; set; }
    [StringLength(9)] public string? Cep { get; set; }
    [StringLength(20)] public string? Numero { get; set; }
    [StringLength(300)] public string? Endereco { get; set; }
    [StringLength(20)]
    public string? Documento { get; set; }
    [EmailAddress, StringLength(256)]
    public string? Email { get; set; }
    [Phone, StringLength(30)]
    public string? Telefone { get; set; }
    public bool Ativo { get; set; } = true;
}
