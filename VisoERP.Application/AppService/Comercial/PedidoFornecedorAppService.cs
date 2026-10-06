using AutoMapper;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Comercial;

namespace VisoERP.Application.AppService.Comercial;

public sealed class PedidoFornecedorAppService(IRepository<Fornecedor> fornecedores,
    IRepository<Suprimento> suprimentos, IRepository<PedidoFornecedor> pedidos,
    IPedidoFornecedorRepository consulta, IUnitOfWork unitOfWork, IMapper mapper) : IPedidoFornecedorAppService
{
    public async Task<Guid> CriarAsync(CriarPedidoDto entrada, CancellationToken cancellationToken)
    {
        if (entrada.Itens is null || entrada.Itens.Count == 0)
            throw new ArgumentException("Informe ao menos um item.");
        if (await fornecedores.ObterPorIdAsync(entrada.FornecedorId, cancellationToken) is null)
            throw new KeyNotFoundException("Fornecedor não encontrado.");
        var pedido = PedidoFornecedor.Criar(entrada.FornecedorId, entrada.DataPedido);
        foreach (var item in entrada.Itens)
        {
            if (await suprimentos.ObterPorIdAsync(item.SuprimentoId, cancellationToken) is not { Ativo: true })
                throw new ArgumentException("Suplemento alimentar inexistente ou inativo.");
            pedido.AdicionarItem(item.SuprimentoId, item.Quantidade, item.PrecoCatalogo, item.PrecoComDesconto);
        }
        await pedidos.AdicionarAsync(pedido, cancellationToken);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
        return pedido.Id;
    }

    public async Task<IReadOnlyList<PedidoFornecedorDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<PedidoFornecedorDto>>(await consulta.ListarComItensAsync(cancellationToken));

    public async Task<PedidoFornecedorDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var pedido = await consulta.ObterComItensAsync(id, cancellationToken);
        return pedido is null ? null : mapper.Map<PedidoFornecedorDto>(pedido);
    }

    public async Task AtualizarAsync(Guid id, CriarPedidoDto entrada, CancellationToken cancellationToken)
    {
        var pedido = await consulta.ObterComItensAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Pedido não encontrado.");
        if (entrada.Itens.Count == 0) throw new ArgumentException("Informe ao menos um item.");
        if (await fornecedores.ObterPorIdAsync(entrada.FornecedorId, cancellationToken) is null) throw new KeyNotFoundException("Fornecedor não encontrado.");
        foreach (var item in entrada.Itens)
            if (await suprimentos.ObterPorIdAsync(item.SuprimentoId, cancellationToken) is not { Ativo: true }) throw new ArgumentException("Suplemento alimentar inexistente ou inativo.");
        pedido.Editar(entrada.FornecedorId, entrada.DataPedido);
        var itensAtuais = pedido.Itens.OrderBy(x => x.SuprimentoId).ToList();
        var novosItens = entrada.Itens.OrderBy(x => x.SuprimentoId).ToList();
        var itensMudaram = itensAtuais.Count != novosItens.Count || itensAtuais.Zip(novosItens).Any(x =>
            x.First.SuprimentoId != x.Second.SuprimentoId ||
            x.First.Quantidade != x.Second.Quantidade ||
            x.First.PrecoCatalogo != x.Second.PrecoCatalogo ||
            x.First.PrecoComDesconto != x.Second.PrecoComDesconto);
        if (!itensMudaram && pedido.FornecedorId == entrada.FornecedorId && entrada.DataPedido.HasValue)
        {
            if (!await consulta.AtualizarDataAsync(id, entrada.DataPedido.Value, cancellationToken))
                throw new InvalidOperationException("O pedido não está mais pendente ou não foi encontrado.");
            return;
        }
        if (itensMudaram)
            pedido.SubstituirItens(entrada.Itens.Select(x => (x.SuprimentoId, x.Quantidade, x.PrecoCatalogo, x.PrecoComDesconto)));
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task CancelarAsync(Guid id, CancellationToken cancellationToken)
    {
        var pedido = await consulta.ObterComItensAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado.");
        pedido.Cancelar();
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }

    public async Task RegistrarFaltaAsync(Guid id, Guid suplementoId, decimal quantidade, CancellationToken cancellationToken)
    {
        var pedido = await consulta.ObterComItensAsync(id, cancellationToken) ?? throw new KeyNotFoundException();
        pedido.RegistrarFalta(suplementoId, quantidade);
        await unitOfWork.SalvarAlteracoesAsync(cancellationToken);
    }
}
