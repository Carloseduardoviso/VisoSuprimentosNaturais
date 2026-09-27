using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Application.AutoMapper.Cadastros;

public sealed class CategoriaProfile : Profile
{
    public CategoriaProfile() => CreateMap<Categoria, CategoriaDto>();
}
