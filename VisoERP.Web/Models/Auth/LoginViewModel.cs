using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Auth;

public sealed class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)]
    public string Senha { get; set; } = string.Empty;
    public bool Lembrar { get; set; }
}
