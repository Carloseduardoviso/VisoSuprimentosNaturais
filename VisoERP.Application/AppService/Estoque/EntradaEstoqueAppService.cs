using AutoMapper;
using VisoERP.Application.DTOs.Estoque;
using VisoERP.Application.Interface.Estoque;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Entities.Financeiro;
using VisoERP.Domain.Enums;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Estoque;

namespace VisoERP.Application.AppService.Estoque;

public sealed class EntradaEstoqueAppService(IRepository<Suprimento> suprimentos,
    IRepository<EntradaEstoque> entradas, IRepository<EstoqueProduto> saldos,
    IRepository<LoteEstoque> lotes, IRepository<MovimentacaoEstoque> movimentacoes,
    IRepository<ContaPagar> contasPagar,
    IEstoqueRepository estoque, IPedidoFornecedorRepository pedidos,
    IUnitOfWork unitOfWork, IMapper mapper) : IEntradaEstoqueAppService
{
    public async Task<Guid> ConfirmarAsync(CriarEntradaDto dto, CancellationToken cancellationToken)
    {
        if (dto.Itens is null || dto.Itens.Count == 0)
            throw new ArgumentException("Informe ao menos um suplemento alimentar.");
        var entrada = EntradaEstoque.Criar(dto.PedidoFornecedorId);
        foreach (var item in dto.Itens)
            entrada.AdicionarItem(item.SuprimentoId, item.Quantidade,
                item.Promocional ? 0 : item.CustoUnitario,
                item.Data, item.Promocional, item.CodigoLote, item.Validade);
        entrada.Validar();

        await unitOfWork.ExecutarEmTransacaoAsync(async ct =>
        {
            var pedido = dto.PedidoFornecedorId.HasValue
                ? await pedidos.ObterComItensAsync(dto.PedidoFornecedorId.Value, ct)
                    ?? throw new KeyNotFoundException("Pedido não encontrado.")
                : null;
            foreach (var item in entrada.Itens)
            {
                if (await suprimentos.ObterPorIdAsync(item.SuprimentoId, ct) is not { Ativo: true })
                    throw new ArgumentException("Suplemento alimentar inexistente ou inativo.");
                pedido?.Receber(item.SuprimentoId, item.Quantidade);

                var saldo = await estoque.ObterSaldoAsync(item.SuprimentoId, ct);
                if (saldo is null)
                {
                    saldo = EstoqueProduto.Criar(item.SuprimentoId);
                    await saldos.AdicionarAsync(saldo, ct);
                }
                saldo.RegistrarEntrada(item.Quantidade, item.CustoUnitario);

                Guid? loteId = null;
                if (item.CodigoLote is not null)
                {
                    var lote = await estoque.ObterLoteAsync(item.SuprimentoId, item.CodigoLote, ct);
                    if (lote is null)
                    {
                        lote = LoteEstoque.Criar(item.SuprimentoId, item.CodigoLote, item.Validade);
                        await lotes.AdicionarAsync(lote, ct);
                    }
                    else if (lote.Validade != item.Validade)
                        throw new InvalidOperationException("A validade do lote diverge do cadastro existente.");
                    lote.RegistrarEntrada(item.Quantidade);
                    loteId = lote.Id;
                }
                await movimentacoes.AdicionarAsync(MovimentacaoEstoque.Criar(item.SuprimentoId,
                    loteId, entrada.Id, pedido is null ? TipoMovimentacaoEstoque.EntradaDireta : TipoMovimentacaoEstoque.RecebimentoPedido, item.Quantidade,
                    item.CustoUnitario, item.Data), ct);
            }
            await entradas.AdicionarAsync(entrada, ct);
            var valorCompra = Math.Round(entrada.TotalCusto, 2, MidpointRounding.AwayFromZero);
            if (valorCompra > 0)
                await contasPagar.AdicionarAsync(ContaPagar.Criar(entrada.Id,
                    pedido?.FornecedorId, "Compra de suplementos alimentares", valorCompra,
                    dto.VencimentoPagamento ?? DateOnly.FromDateTime(DateTime.Today)), ct);
        }, cancellationToken);
        return entrada.Id;
    }

    public async Task<IReadOnlyList<EntradaEstoqueDto>> ListarAsync(CancellationToken cancellationToken)
    {
        var entradasDto = mapper.Map<List<EntradaEstoqueDto>>(
            await estoque.ListarEntradasComItensAsync(cancellationToken));
        var precosCatalogo = (await suprimentos.ListarAsync(cancellationToken))
            .ToDictionary(x => x.Id, x => x.PrecoCatalogo);
        return entradasDto.Select(entrada => entrada with
        {
            TotalCatalogo = entrada.Itens.Sum(item => item.Quantidade *
                (precosCatalogo.TryGetValue(item.SuprimentoId, out var preco) ? preco : item.CustoUnitario))
        }).ToList();
    }

    public async Task<IReadOnlyList<SaldoEstoqueDto>> ListarSaldosAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<SaldoEstoqueDto>>(await estoque.ListarSaldosAsync(cancellationToken));

    public async Task AtualizarDataAsync(Guid id, DateTimeOffset data, CancellationToken cancellationToken)
    {
        if (!await estoque.AtualizarDataEntradaAsync(id, data, cancellationToken))
            throw new KeyNotFoundException("Entrada não encontrada.");
    }

    public async Task ExcluirAsync(Guid id, CancellationToken cancellationToken)
    {
        var entrada = (await estoque.ListarEntradasComItensAsync(cancellationToken)).SingleOrDefault(x => x.Id == id)
            ?? throw new KeyNotFoundException("Entrada não encontrada.");
        var conta = (await contasPagar.ListarAsync(cancellationToken)).SingleOrDefault(x => x.EntradaEstoqueId == id);
        if (conta?.ValorPago > 0)
            throw new InvalidOperationException("A entrada não pode ser excluída porque a conta a pagar já possui pagamento.");
        var movimentos = (await movimentacoes.ListarAsync(cancellationToken)).Where(x => x.ReferenciaId == id).ToList();
        await unitOfWork.ExecutarEmTransacaoAsync(async ct =>
        {
            foreach (var item in entrada.Itens)
            {
                var saldo = await estoque.ObterSaldoAsync(item.SuprimentoId, ct)
                    ?? throw new InvalidOperationException("Saldo de estoque não encontrado.");
                saldo.RegistrarSaida(item.Quantidade);
                if (!string.IsNullOrWhiteSpace(item.CodigoLote))
                {
                    var lote = await estoque.ObterLoteAsync(item.SuprimentoId, item.CodigoLote, ct)
                        ?? throw new InvalidOperationException("Lote de estoque não encontrado.");
                    lote.RegistrarSaida(item.Quantidade);
                }
                if (entrada.PedidoFornecedorId.HasValue)
                {
                    var pedido = await pedidos.ObterComItensAsync(entrada.PedidoFornecedorId.Value, ct)
                        ?? throw new KeyNotFoundException("Pedido não encontrado.");
                    pedido.EstornarRecebimento(item.SuprimentoId, item.Quantidade);
                }
            }
            foreach (var movimento in movimentos) movimentacoes.Excluir(movimento);
            if (conta is not null) contasPagar.Excluir(conta);
            entradas.Excluir(entrada);
            await unitOfWork.SalvarAlteracoesAsync(ct);
        }, cancellationToken);
    }
}
