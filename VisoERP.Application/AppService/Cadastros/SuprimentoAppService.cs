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
        if (string.IsNullOrWhiteSpace(entrada.CodigoInterno))
            throw new ArgumentException("Código interno é obrigatório.", nameof(entrada));
        if (entrada.CategoriaId is Guid categoriaId &&
            await categorias.ObterPorIdAsync(categoriaId, cancellationToken) is null)
            throw new ArgumentException("Categoria não encontrada.", nameof(entrada));
        if (await suprimentos.CodigoExisteAsync(entrada.CodigoInterno.Trim(), id, cancellationToken))
            throw new ArgumentException("Código interno já cadastrado.", nameof(entrada));

        Suprimento suprimento;
        if (id.HasValue)
        {
            suprimento = await suprimentos.ObterPorIdAsync(id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Suprimento não encontrado.");
        }
        else
        {
            suprimento = Suprimento.Criar(entrada.CodigoInterno, entrada.Nome,
                entrada.PrecoCatalogo, entrada.PrecoComDesconto, entrada.QuantidadeMinimaCompra);
            await suprimentos.AdicionarAsync(suprimento, cancellationToken);
        }

        suprimento.Atualizar(entrada.CodigoInterno, entrada.Nome, entrada.PrecoCatalogo,
            entrada.PrecoComDesconto, entrada.QuantidadeMinimaCompra, entrada.EstoqueMinimo,
            entrada.CategoriaId, entrada.Descricao, entrada.FormaDeUso,
            entrada.ImagemCaminho ?? suprimento.ImagemCaminho, entrada.Ativo);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return suprimento.Id;
    }
}
