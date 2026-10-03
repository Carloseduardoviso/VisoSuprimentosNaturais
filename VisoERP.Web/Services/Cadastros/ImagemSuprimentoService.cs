namespace VisoERP.Web.Services.Cadastros;

public sealed class ImagemSuprimentoService(IWebHostEnvironment environment)
{
    private const long TamanhoMaximo = 2 * 1024 * 1024;
    private static readonly HashSet<string> Extensoes = [".jpg", ".jpeg", ".png", ".webp"];

    private string Diretorio => Path.Combine(environment.ContentRootPath, "App_Data", "Suprimentos");

    public async Task<string> SalvarAsync(IFormFile arquivo, CancellationToken cancellationToken)
    {
        var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
        if (!Extensoes.Contains(extensao) || arquivo.Length is <= 0 or > TamanhoMaximo)
            throw new ArgumentException("Imagem inválida. Use JPG, PNG ou WebP com até 2 MB.");

        await using var origem = arquivo.OpenReadStream();
        var cabecalho = new byte[12];
        if (await origem.ReadAsync(cabecalho, cancellationToken) < 12 || !AssinaturaValida(cabecalho, extensao))
            throw new ArgumentException("O conteúdo do arquivo não corresponde ao formato informado.");
        origem.Position = 0;

        Directory.CreateDirectory(Diretorio);
        var nome = Guid.NewGuid().ToString("N") + extensao;
        var caminho = Path.Combine(Diretorio, nome);
        try
        {
            await using var destino = new FileStream(caminho, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 81920, useAsync: true);
            await origem.CopyToAsync(destino, cancellationToken);
        }
        catch
        {
            if (File.Exists(caminho)) File.Delete(caminho);
            throw;
        }
        return nome;
    }

    public string? CaminhoParaLeitura(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || Path.GetFileName(nome) != nome) return null;
        var caminho = Path.Combine(Diretorio, nome);
        return File.Exists(caminho) ? caminho : null;
    }

    public void Excluir(string nome)
    {
        var caminho = CaminhoParaLeitura(nome);
        if (caminho is not null) File.Delete(caminho);
    }

    private static bool AssinaturaValida(byte[] bytes, string extensao) => extensao switch
    {
        ".png" => bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
        ".jpg" or ".jpeg" => bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
        ".webp" => bytes.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                   bytes.AsSpan(8, 4).SequenceEqual("WEBP"u8),
        _ => false
    };
}
