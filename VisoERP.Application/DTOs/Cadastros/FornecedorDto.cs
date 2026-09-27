namespace VisoERP.Application.DTOs.Cadastros;

public sealed record FornecedorDto(Guid Id, string Nome, string? Documento, string? Email,
    string? Telefone, bool Ativo);

public sealed record SalvarFornecedorDto(string Nome, string? Documento, string? Email,
    string? Telefone, bool Ativo);
