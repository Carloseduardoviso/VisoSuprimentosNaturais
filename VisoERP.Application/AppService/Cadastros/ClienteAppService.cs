using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Application.Interface.Cadastros;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories;

namespace VisoERP.Application.AppService.Cadastros;

public sealed class ClienteAppService(IRepository<Cliente> clientes,
    IUnitOfWork unitOfWork, IMapper mapper) : IClienteAppService
{
    public async Task<IReadOnlyList<ClienteDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<ClienteDto>>((await clientes.ListarAsync(cancellationToken))
            .OrderBy(x => x.Nome).ToList());

    public async Task<ClienteDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var cliente = await clientes.ObterPorIdAsync(id, cancellationToken);
        return cliente is null ? null : mapper.Map<ClienteDto>(cliente);
    }

    public async Task<Guid> SalvarAsync(Guid? id, SalvarClienteDto entrada, CancellationToken cancellationToken)
    {
        var documento = entrada.Documento?.Trim();
        if (!string.IsNullOrWhiteSpace(documento) &&
            await clientes.ExisteAsync(x => x.Documento == documento && x.Id != id, cancellationToken))
            throw new ArgumentException("Documento já cadastrado para outro cliente.", nameof(entrada));

        Cliente cliente;
        if (id.HasValue)
        {
            cliente = await clientes.ObterPorIdAsync(id.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Cliente não encontrado.");
            cliente.Atualizar(entrada.Nome, documento, entrada.Email, entrada.Telefone, entrada.Ativo);
        }
        else
        {
            cliente = Cliente.Criar(entrada.Nome, documento, entrada.Email, entrada.Telefone);
            cliente.Atualizar(entrada.Nome, documento, entrada.Email, entrada.Telefone, entrada.Ativo);
            await clientes.AdicionarAsync(cliente, cancellationToken);
        }
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return cliente.Id;
    }
}
