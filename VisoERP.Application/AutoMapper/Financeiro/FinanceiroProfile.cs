using AutoMapper;
using VisoERP.Application.DTOs.Financeiro;
using VisoERP.Domain.Entities.Financeiro;

namespace VisoERP.Application.AutoMapper.Financeiro;

public sealed class FinanceiroProfile : Profile
{
    public FinanceiroProfile()
    {
        CreateMap<Investimento, InvestimentoDto>();
        CreateMap<Despesa, DespesaDto>();
        CreateMap<ContaPagar, ContaPagarDto>();
    }
}
