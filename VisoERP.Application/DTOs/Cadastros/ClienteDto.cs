namespace VisoERP.Application.DTOs.Cadastros;

public sealed record ClienteDto(Guid Id, string Nome, string? Documento, string? Email,
    string? Telefone, bool Ativo, string? Cep = null, string? Numero = null, string? Endereco = null);

public sealed record SalvarClienteDto(string Nome, string? Documento, string? Email,
    string? Telefone, bool Ativo, string? Cep = null, string? Numero = null, string? Endereco = null);
