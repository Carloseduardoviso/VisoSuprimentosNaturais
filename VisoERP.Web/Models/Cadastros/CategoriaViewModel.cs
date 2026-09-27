using System.ComponentModel.DataAnnotations;

namespace VisoERP.Web.Models.Cadastros;

public sealed class CategoriaViewModel
{
    public Guid? Id { get; set; }
    [Required, StringLength(120)]
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
}
