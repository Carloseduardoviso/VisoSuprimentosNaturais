using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using VisoERP.Application.AppService.Cadastros;
using VisoERP.Application.DependencyInjection;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Domain.Entities.Cadastros;
using VisoERP.Domain.Interfaces.Repositories;
using VisoERP.Domain.Interfaces.Repositories.Cadastros;

namespace VisoERP.Tests;

public class SuprimentoAppServiceTests
{
    [Fact]
    public async Task CadastroPersisteEDevolveDadosMapeados()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.RegistrarApplication(null);
        using var provider = services.BuildServiceProvider();
        var repositorio = new SuprimentosEmMemoria();
        var unidade = new UnidadeDeTeste();
        var service = new SuprimentoAppService(repositorio, new CategoriasEmMemoria(),
            unidade, provider.GetRequiredService<IMapper>());

        var id = await service.SalvarAsync(null,
            new SalvarSuprimentoDto("CAM-001", "Camomila", 10m, 8m, 2, 3,
                null, "Erva", "Infusão", null, true), CancellationToken.None);
        var produtos = await service.ListarAsync(CancellationToken.None);

        Assert.Equal(id, Assert.Single(produtos).Id);
        Assert.Equal("CAM-001", produtos[0].CodigoInterno);
        Assert.Equal(2, produtos[0].QuantidadeMinimaCompra);
        Assert.Equal(1, unidade.Salvamentos);
    }

    private sealed class SuprimentosEmMemoria : ISuprimentoRepository
    {
        private readonly List<Suprimento> _itens = [];
        public Task<Suprimento?> ObterPorIdAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(_itens.FirstOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<Suprimento>> ListarAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Suprimento>>(_itens);
        public Task<bool> CodigoExisteAsync(string codigo, Guid? ignorarId, CancellationToken ct) =>
            Task.FromResult(_itens.Any(x => x.CodigoInterno == codigo && x.Id != ignorarId));
        public Task AdicionarAsync(Suprimento item, CancellationToken ct)
        {
            _itens.Add(item);
            return Task.CompletedTask;
        }
    }

    private sealed class CategoriasEmMemoria : IRepository<Categoria>
    {
        public Task<Categoria?> ObterPorIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult<Categoria?>(null);
        public Task<IReadOnlyList<Categoria>> ListarAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Categoria>>([]);
        public Task<bool> ExisteAsync(System.Linq.Expressions.Expression<Func<Categoria, bool>> criterio,
            CancellationToken ct = default) => Task.FromResult(false);
        public Task AdicionarAsync(Categoria item, CancellationToken ct = default) => Task.CompletedTask;
        public void Atualizar(Categoria item) { }
    }

    private sealed class UnidadeDeTeste : IUnitOfWork
    {
        public int Salvamentos { get; private set; }
        public Task<int> SalvarAlteracoesAsync(CancellationToken ct = default)
        {
            Salvamentos++;
            return Task.FromResult(1);
        }
        public async Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> operacao,
            CancellationToken ct = default) => await operacao(ct);
    }
}
