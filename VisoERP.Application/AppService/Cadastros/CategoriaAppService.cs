using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories;

namespace VisoERP.Application.AppService.Cadastros;

public sealed class CategoriaAppService(IRepository<Categoria> categorias,
    IUnitOfWork unitOfWork, IMapper mapper) : ICategoriaAppService
{
    public async Task<IReadOnlyList<CategoriaDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<CategoriaDto>>((await categorias.ListarAsync(cancellationToken))
            .OrderBy(x => x.Nome).ToList());

    public async Task<CategoriaDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var categoria = await categorias.ObterPorIdAsync(id, cancellationToken);
        return categoria is null ? null : mapper.Map<CategoriaDto>(categoria);
    }

    public async Task<Guid> SalvarAsync(Guid? id, SalvarCategoriaDto entrada, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entrada.Nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(entrada));
        var nome = entrada.Nome.Trim();
        if (await categorias.ExisteAsync(x => x.Nome == nome && x.Id != id, cancellationToken))
            throw new ArgumentException("Categoria já cadastrada.", nameof(entrada));

        Categoria categoria;
        if (id.HasValue)
        {
            categoria = await categorias.ObterPorIdAsync(id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Categoria não encontrada.");
            categoria.Atualizar(nome, entrada.Ativa);
        }
        else
        {
            categoria = Categoria.Criar(nome);
            categoria.Atualizar(nome, entrada.Ativa);
            await categorias.AdicionarAsync(categoria, cancellationToken);
        }
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return categoria.Id;
    }
}
