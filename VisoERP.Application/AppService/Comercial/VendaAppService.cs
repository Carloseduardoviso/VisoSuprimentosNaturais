using AutoMapper;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Application.Interface.Comercial;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Entities.Comercial;
using VisoERP.Domain.Entities.Estoque;
using VisoERP.Domain.Enums;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Comercial;
using VisoERP.Domain.Interfaces.Repositories.Estoque;

namespace VisoERP.Application.AppService.Comercial;

public sealed class VendaAppService(IRepository<Cliente> clientes,
    IRepository<Suprimento> suprimentos, IRepository<Venda> vendas,
    IRepository<RecebimentoVenda> recebimentos, IRepository<MovimentacaoEstoque> movimentacoes,
    IEstoqueRepository estoque, IVendaRepository consulta,
    IUnitOfWork unitOfWork, IMapper mapper) : IVendaAppService
{
    public async Task<Guid> SalvarRascunhoAsync(CriarVendaDto dto, CancellationToken ct)
    {
        if (dto.ClienteId == Guid.Empty) throw new ArgumentException("Cliente obrigatório.");
        var venda = Venda.Criar(dto.ClienteId, dto.Desconto, dto.ValorEntrada,
            dto.NumeroParcelas, dto.PrimeiroVencimento);
        AdicionarItensDeRascunho(venda, dto);
        await unitOfWork.ExecutarEmTransacaoAsync(async token =>
        {
            if (await clientes.ObterPorIdAsync(dto.ClienteId, token) is not { Ativo: true })
                throw new ArgumentException("Cliente inexistente ou inativo.");
            await vendas.AdicionarAsync(venda, token);
        }, ct);
        return venda.Id;
    }

    public async Task AtualizarRascunhoAsync(Guid id, CriarVendaDto dto, CancellationToken ct)
    {
        var venda = await consulta.ObterComItensEParcelasAsync(id, ct)
            ?? throw new KeyNotFoundException("Venda não encontrada.");
        if (venda.Finalizada) throw new InvalidOperationException("Venda já finalizada.");
        if (await clientes.ObterPorIdAsync(dto.ClienteId, ct) is not { Ativo: true })
            throw new ArgumentException("Cliente inexistente ou inativo.");
        venda.AtualizarRascunho(dto.ClienteId, dto.Desconto, dto.ValorEntrada,
            dto.NumeroParcelas, dto.PrimeiroVencimento, []);
        AdicionarItensDeRascunho(venda, dto);
        await consulta.AtualizarRascunhoAsync(venda, ct);
        await unitOfWork.SalvarAlteracoesAsync(ct);
    }

    public async Task FinalizarAsync(Guid id, CancellationToken ct)
    {
        await unitOfWork.ExecutarEmTransacaoAsync(async token =>
        {
            var venda = await consulta.ObterComItensEParcelasAsync(id, token)
                ?? throw new KeyNotFoundException("Venda não encontrada.");
            if (venda.Finalizada) throw new InvalidOperationException("Venda já finalizada.");
            var custos = new List<(Guid SuprimentoId, decimal Custo)>();
            foreach (var item in venda.Itens)
            {
                var produto = await suprimentos.ObterPorIdAsync(item.SuprimentoId, token)
                    ?? throw new InvalidOperationException("Suplemento alimentar inexistente.");
                var saldo = await estoque.ObterSaldoAsync(item.SuprimentoId, token)
                    ?? throw new InvalidOperationException($"Sem estoque de {produto.Nome}.");
                if (item.Quantidade > saldo.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente de {produto.Nome}.");
                var custo = saldo.RegistrarSaida(item.Quantidade);
                custos.Add((item.SuprimentoId, custo));
                await movimentacoes.AdicionarAsync(MovimentacaoEstoque.Criar(item.SuprimentoId,
                    null, venda.Id, TipoMovimentacaoEstoque.SaidaVenda, item.Quantidade,
                    custo, item.Data), token);
            }
            venda.AplicarCustos(custos);
            venda.Finalizar();
            if (venda.ValorEntrada > 0)
                await recebimentos.AdicionarAsync(RecebimentoVenda.Criar(venda.Id,
                    null, venda.ValorEntrada, venda.Data), token);
        }, ct);
    }

    public async Task ExcluirRascunhoAsync(Guid id, CancellationToken ct)
    {
        var venda = await consulta.ObterComItensEParcelasAsync(id, ct)
            ?? throw new KeyNotFoundException("Venda não encontrada.");
        if (venda.Finalizada) throw new InvalidOperationException("Venda finalizada não pode ser excluída.");
        consulta.Excluir(venda);
        await unitOfWork.SalvarAlteracoesAsync(ct);
    }

    private static void AdicionarItensDeRascunho(Venda venda, CriarVendaDto dto)
    {
        foreach (var item in dto.Itens)
            venda.AdicionarItem(item.SuprimentoId, item.Quantidade, item.PrecoCatalogo,
                item.PrecoUnitario, item.Promocional, 0, item.Data);
    }
    public async Task<Guid> RegistrarAsync(CriarVendaDto dto, CancellationToken cancellationToken)
    {
        if (dto.Itens is null || dto.Itens.Count == 0)
            throw new ArgumentException("Informe ao menos um suplemento alimentar.");
        var venda = Venda.Criar(dto.ClienteId, dto.Desconto, dto.ValorEntrada,
            dto.NumeroParcelas, dto.PrimeiroVencimento);
        await unitOfWork.ExecutarEmTransacaoAsync(async ct =>
        {
            if (await clientes.ObterPorIdAsync(dto.ClienteId, ct) is not { Ativo: true })
                throw new ArgumentException("Cliente inexistente ou inativo.");
            foreach (var item in dto.Itens)
            {
                var produto = await suprimentos.ObterPorIdAsync(item.SuprimentoId, ct);
                if (produto is not { Ativo: true })
                    throw new ArgumentException("Suplemento alimentar inexistente ou inativo.");
                var saldo = await estoque.ObterSaldoAsync(item.SuprimentoId, ct)
                    ?? throw new InvalidOperationException($"Sem estoque de {produto.Nome}.");
                var lotes = await estoque.ListarLotesDisponiveisAsync(item.SuprimentoId, ct);
                var lotesValidos = lotes.Where(x => x.Validade is null ||
                    x.Validade >= DateOnly.FromDateTime(item.Data.Date)).ToList();
                var disponivel = saldo.Quantidade - lotes.Sum(x => x.Quantidade) +
                    lotesValidos.Sum(x => x.Quantidade);
                if (item.Quantidade > disponivel)
                    throw new InvalidOperationException($"Estoque válido insuficiente de {produto.Nome}.");
                var custoHistorico = saldo.RegistrarSaida(item.Quantidade);
                venda.AdicionarItem(item.SuprimentoId, item.Quantidade, item.PrecoCatalogo,
                    item.PrecoUnitario, item.Promocional, custoHistorico, item.Data);

                var restante = item.Quantidade;
                foreach (var lote in lotesValidos)
                {
                    var retirada = Math.Min(restante, lote.Quantidade);
                    if (retirada <= 0) continue;
                    lote.RegistrarSaida(retirada);
                    restante -= retirada;
                    await movimentacoes.AdicionarAsync(MovimentacaoEstoque.Criar(item.SuprimentoId,
                        lote.Id, venda.Id, TipoMovimentacaoEstoque.SaidaVenda,
                        retirada, custoHistorico, item.Data), ct);
                    if (restante == 0) break;
                }
                if (restante > 0)
                    await movimentacoes.AdicionarAsync(MovimentacaoEstoque.Criar(item.SuprimentoId,
                        null, venda.Id, TipoMovimentacaoEstoque.SaidaVenda,
                        restante, custoHistorico, item.Data), ct);
            }
            venda.Finalizar();
            await vendas.AdicionarAsync(venda, ct);
            if (venda.ValorEntrada > 0)
                await recebimentos.AdicionarAsync(RecebimentoVenda.Criar(venda.Id,
                    null, venda.ValorEntrada, venda.Data), ct);
        }, cancellationToken);
        return venda.Id;
    }

    public async Task RegistrarPagamentoAsync(Guid vendaId, Guid parcelaId, decimal valor,
        DateTimeOffset data, CancellationToken cancellationToken)
    {
        await unitOfWork.ExecutarEmTransacaoAsync(async ct =>
        {
            var venda = await consulta.ObterComItensEParcelasAsync(vendaId, ct)
                ?? throw new KeyNotFoundException("Venda não encontrada.");
            venda.RegistrarPagamento(parcelaId, valor, data);
            await recebimentos.AdicionarAsync(RecebimentoVenda.Criar(vendaId,
                parcelaId, valor, data), ct);
        }, cancellationToken);
    }

    public async Task<VendaDto?> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        var venda = await consulta.ObterComItensEParcelasAsync(id, cancellationToken);
        return venda is null ? null : mapper.Map<VendaDto>(venda);
    }

    public async Task<IReadOnlyList<VendaDto>> ListarAsync(CancellationToken cancellationToken) =>
        mapper.Map<List<VendaDto>>(await consulta.ListarComItensEParcelasAsync(cancellationToken));
}
