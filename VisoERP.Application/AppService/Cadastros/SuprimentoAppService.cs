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
                ?? throw new KeyNotFoundException("Suplemento alimentar não encontrado.")
            : null;
        var codigoInterno = string.IsNullOrWhiteSpace(entrada.CodigoInterno)
            ? suprimentoExistente?.CodigoInterno ?? await GerarCodigoInternoAsync(cancellationToken)
            : entrada.CodigoInterno.Trim();
        if (entrada.CategoriaId is Guid categoriaId &&
            await categorias.ObterPorIdAsync(categoriaId, cancellationToken) is null)
            throw new ArgumentException("Categoria não encontrada.", nameof(entrada));
        if (await suprimentos.CodigoExisteAsync(codigoInterno, id, cancellationToken))
            throw new ArgumentException("Código interno já cadastrado.", nameof(entrada));

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
