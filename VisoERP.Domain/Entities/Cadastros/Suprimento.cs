namespace VisoERP.Domain.Entities.Cadastros;

public sealed class Suprimento : EntidadeBase
{
    private Suprimento() { }

    public string CodigoInterno { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public decimal PrecoCatalogo { get; private set; }
    public decimal PrecoComDesconto { get; private set; }
    public string? Descricao { get; private set; }
    public string? FormaDeUso { get; private set; }
    public string? ImagemCaminho { get; private set; }
    public int QuantidadeMinimaCompra { get; private set; }
    public int EstoqueMinimo { get; private set; }
    public bool Ativo { get; private set; } = true;
    public Guid? CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }

    public static Suprimento Criar(string codigoInterno, string nome, decimal precoCatalogo,
        decimal precoComDesconto, int quantidadeMinimaCompra)
    {
        var suprimento = new Suprimento();
        suprimento.Atualizar(codigoInterno, nome, precoCatalogo, precoComDesconto,
            quantidadeMinimaCompra, 0, null, null, null, null, true);
        return suprimento;
    }

    public void Atualizar(string codigoInterno, string nome, decimal precoCatalogo,
        decimal precoComDesconto, int quantidadeMinimaCompra, int estoqueMinimo,
        Guid? categoriaId, string? descricao, string? formaDeUso, string? imagemCaminho, bool ativo)
    {
        if (string.IsNullOrWhiteSpace(codigoInterno) || codigoInterno.Trim().Length > 50)
            throw new ArgumentException("Código interno deve ter de 1 a 50 caracteres.", nameof(codigoInterno));
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length > 200)
            throw new ArgumentException("Nome deve ter de 1 a 200 caracteres.", nameof(nome));
        if (precoCatalogo < 0) throw new ArgumentOutOfRangeException(nameof(precoCatalogo));
        if (precoComDesconto < 0 || precoComDesconto > precoCatalogo)
            throw new ArgumentOutOfRangeException(nameof(precoComDesconto));
        if (quantidadeMinimaCompra < 1) throw new ArgumentOutOfRangeException(nameof(quantidadeMinimaCompra));
        if (estoqueMinimo < 0) throw new ArgumentOutOfRangeException(nameof(estoqueMinimo));
        if (descricao?.Length > 2000) throw new ArgumentException("Descrição muito longa.", nameof(descricao));
        if (formaDeUso?.Length > 2000) throw new ArgumentException("Forma de uso muito longa.", nameof(formaDeUso));

        CodigoInterno = codigoInterno.Trim();
        Nome = nome.Trim();
        PrecoCatalogo = precoCatalogo;
        PrecoComDesconto = precoComDesconto;
        QuantidadeMinimaCompra = quantidadeMinimaCompra;
        EstoqueMinimo = estoqueMinimo;
        CategoriaId = categoriaId;
        Descricao = descricao?.Trim();
        FormaDeUso = formaDeUso?.Trim();
        ImagemCaminho = imagemCaminho;
        Ativo = ativo;
    }
}
