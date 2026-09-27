using AutoMapper;
using VisoERP.Application.DTOs.Estoque;
using VisoERP.Domain.Entities.Estoque;

namespace VisoERP.Application.AutoMapper.Estoque;

public sealed class EntradaEstoqueProfile : Profile
{
    public EntradaEstoqueProfile()
    {
        CreateMap<EstoqueProduto, SaldoEstoqueDto>();
        CreateMap<ItemEntradaEstoque, ItemEntradaDto>();
        CreateMap<EntradaEstoque, EntradaEstoqueDto>();
    }
}
