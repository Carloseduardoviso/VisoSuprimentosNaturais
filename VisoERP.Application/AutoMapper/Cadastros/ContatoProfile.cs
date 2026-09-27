using AutoMapper;
using VisoERP.Application.DTOs.Cadastros;
using VisoERP.Domain.Entities.Cadastros;

namespace VisoERP.Application.AutoMapper.Cadastros;

public sealed class ContatoProfile : Profile
{
    public ContatoProfile()
    {
        CreateMap<Cliente, ClienteDto>();
        CreateMap<Fornecedor, FornecedorDto>();
    }
}
