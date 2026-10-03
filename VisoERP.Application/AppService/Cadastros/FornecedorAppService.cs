using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories;

namespace VisoERP.Application.AppService.Cadastros;

public sealed class FornecedorAppService(IRepository<Fornecedor> fornecedores,
    IUnitOfWork unitOfWork, IMapper mapper) : IFornecedorAppService
{
    public async Task<IReadOnlyList<FornecedorDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<FornecedorDto>>((await fornecedores.ListarAsync(cancellationToken))
            .OrderBy(x => x.Nome).ToList());

    public async Task<FornecedorDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var fornecedor = await fornecedores.ObterPorIdAsync(id, cancellationToken);
        return fornecedor is null ? null : mapper.Map<FornecedorDto>(fornecedor);
    }

    public async Task<Guid> SalvarAsync(Guid? id, SalvarFornecedorDto entrada, CancellationToken cancellationToken)
    {
        var fornecedorExistente = id.HasValue
            ? await fornecedores.ObterPorIdAsync(id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Fornecedor n�o encontrado.")
            : null;
        var documento = entrada.Documento?.Trim() ?? fornecedorExistente?.Documento;
        var email = entrada.Email?.Trim() ?? fornecedorExistente?.Email;
        if (!string.IsNullOrWhiteSpace(documento) &&
            await fornecedores.ExisteAsync(x => x.Documento == documento && x.Id != id, cancellationToken))
            throw new ArgumentException("Documento j� cadastrado para outro fornecedor.", nameof(entrada));

        Fornecedor fornecedor;
        if (id.HasValue)
        {
            fornecedor = fornecedorExistente!;
            fornecedor.Atualizar(entrada.Nome, documento, email, entrada.Telefone, entrada.Ativo);
        }
        else
        {
            fornecedor = Fornecedor.Criar(entrada.Nome, documento, email, entrada.Telefone);
            fornecedor.Atualizar(entrada.Nome, documento, email, entrada.Telefone, entrada.Ativo);
            await fornecedores.AdicionarAsync(fornecedor, cancellationToken);
        }
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return fornecedor.Id;
    }
}
