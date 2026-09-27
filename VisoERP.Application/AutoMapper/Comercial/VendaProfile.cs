using AutoMapper;
using VisoERP.Application.DTOs.Comercial;
using VisoERP.Domain.Entities.Comercial;

namespace VisoERP.Application.AutoMapper.Comercial;

public sealed class VendaProfile : Profile
{
    public VendaProfile()
    {
        CreateMap<ItemVenda, ItemVendaDto>();
        CreateMap<ParcelaVenda, ParcelaVendaDto>();
        CreateMap<Venda, VendaDto>();
    }
}
