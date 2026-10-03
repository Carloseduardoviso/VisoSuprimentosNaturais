using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Cadastros;

namespace VisoERP.Application.AppService.Cadastros;

public sealed class SuprimentoAppService(ISuprimentoRepository suprimentos,
    IRepository<Categoria> categorias, IUnitOfWork unitOfWork, IMapper mapper) : ISuprimentoAppService
{
    public async Task<IReadOnlyList<SuprimentoDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<SuprimentoDto>>(await suprimentos.ListarAsync(cancellationToken));

    public async Task<SuprimentoDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var suprimento = await suprimentos.ObterPorIdAsync(id, cancellationToken);
        return suprimento is null ? null : mapper.Map<SuprimentoDto>(suprimento);
    }

    public async Task<IReadOnlyList<CategoriaDto>> ListarCategoriasAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<CategoriaDto>>(await categorias.ListarAsync(cancellationToken));

    public async Task<Guid> SalvarAsync(Guid? id, SalvarSuprimentoDto entrada, CancellationToken cancellationToken)
    {
        var suprimentoExistente = id.HasValue
            ? await suprimentos.ObterPorIdAsync(id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Suplemento alimentar n�o encontrado.")
            : null;
        var codigoInterno = string.IsNullOrWhiteSpace(entrada.CodigoInterno)
            ? suprimentoExistente?.CodigoInterno ?? await GerarCodigoInternoAsync(cancellationToken)
            : entrada.CodigoInterno.Trim();
        if (entrada.CategoriaId is Guid categoriaId &&
            await categorias.ObterPorIdAsync(categoriaId, cancellationToken) is null)
            throw new ArgumentException("Categoria n�o encontrada.", nameof(entrada));
        if (await suprimentos.CodigoExisteAsync(codigoInterno, id, cancellationToken))
            throw new ArgumentException("C�digo interno j� cadastrado.", nameof(entrada));

        Suprimento suprimento;
        if (id.HasValue)
        {
            suprimento = suprimentoExistente!;
        }
        else
        {
            suprimento = Suprimento.Criar(codigoInterno, entrada.Nome,
                entrada.PrecoCatalogo, entrada.PrecoComDesconto, entrada.QuantidadeMinimaCompra);
            await suprimentos.AdicionarAsync(suprimento, cancellationToken);
        }

        suprimento.Atualizar(codigoInterno, entrada.Nome, entrada.PrecoCatalogo,
            entrada.PrecoComDesconto, entrada.QuantidadeMinimaCompra, entrada.EstoqueMinimo,
            entrada.CategoriaId, entrada.Descricao, entrada.FormaDeUso,
            entrada.ImagemCaminho ?? suprimento.ImagemCaminho, entrada.Ativo);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return suprimento.Id;
    }

    public async Task AplicarDescontoCategoriaAsync(Guid categoriaId, decimal percentual, CancellationToken cancellationToken)
    {
        if (percentual is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(percentual));
        if (await categorias.ObterPorIdAsync(categoriaId, cancellationToken) is null)
            throw new KeyNotFoundException("Categoria n�o encontrada.");
        var produtos = (await suprimentos.ListarAsync(cancellationToken)).Where(x => x.CategoriaId == categoriaId).ToList();
        foreach (var produto in produtos)
            produto.Atualizar(produto.CodigoInterno, produto.Nome, produto.PrecoCatalogo,
                Math.Round(produto.PrecoCatalogo * (1 - percentual / 100), 2), produto.QuantidadeMinimaCompra,
                produto.EstoqueMinimo, produto.CategoriaId, produto.Descricao, produto.FormaDeUso,
                produto.ImagemCaminho, produto.Ativo);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    private async Task<string> GerarCodigoInternoAsync(CancellationToken cancellationToken)
    {
        string codigo;
        do
        {
            codigo = $"SUP-{Guid.NewGuid():N}"[..12].ToUpperInvariant();
        } while (await suprimentos.CodigoExisteAsync(codigo, null, cancellationToken));
        return codigo;
    }
}
