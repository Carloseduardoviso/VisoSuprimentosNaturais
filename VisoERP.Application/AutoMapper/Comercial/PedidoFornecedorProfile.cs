using AutoMapper;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Application.AutoMapper.Comercial;

public sealed class PedidoFornecedorProfile : Profile
{
    public PedidoFornecedorProfile()
    {
        CreateMap<ItemPedidoFornecedor, ItemPedidoDto>();
        CreateMap<PedidoFornecedor, PedidoFornecedorDto>()
            .ForCtorParam("Situacao", x => x.MapFrom(s => s.Situacao.ToString()));
    }
}
