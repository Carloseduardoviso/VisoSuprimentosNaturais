using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Application.AutoMapper.Cadastros;

public sealed class SuprimentoProfile : Profile
{
    public SuprimentoProfile() => CreateMap<Suprimento, SuprimentoDto>();
}
