using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Auth;

public sealed class CadastroUsuarioViewModel
{
    [Required, Display(Name = "Nome completo")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required, Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

    [Required, MinLength(8), DataType(DataType.Password), Display(Name = "Senha")]
    public string Senha { get; set; } = string.Empty;

    [Required, Compare(nameof(Senha)), DataType(DataType.Password), Display(Name = "Confirmar senha")]
    public string ConfirmacaoSenha { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Chave de cadastro")]
    public string Chave { get; set; } = string.Empty;
}
