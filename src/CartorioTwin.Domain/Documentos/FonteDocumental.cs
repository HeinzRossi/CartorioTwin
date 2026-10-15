namespace CartorioTwin.Domain.Documentos;

public sealed record FonteDocumental
{
    private static readonly char[] CaracteresDeCaminho = ['\\', '/', ':', '*', '?', '"', '<', '>', '|'];

    private FonteDocumental(
        FonteDocumentalId id,
        VersaoFonte versao,
        HashSha256 hash,
        string nomeOriginalSanitizado,
        string mimeTypeValidado,
        long tamanhoBytes)
    {
        Id = id;
        Versao = versao;
        Hash = hash;
        NomeOriginalSanitizado = nomeOriginalSanitizado;
        MimeTypeValidado = mimeTypeValidado;
        TamanhoBytes = tamanhoBytes;
    }

    public FonteDocumentalId Id { get; }

    public VersaoFonte Versao { get; }

    public HashSha256 Hash { get; }

    public string NomeOriginalSanitizado { get; }

    public string MimeTypeValidado { get; }

    public long TamanhoBytes { get; }

    public static FonteDocumental Criar(
        FonteDocumentalId id,
        VersaoFonte versao,
        HashSha256 hash,
        string nomeOriginalSanitizado,
        string mimeTypeValidado,
        long tamanhoBytes)
    {
        if (id == default)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador da fonte não pode ser vazio.");
        }

        if (versao == default)
        {
            throw new ArgumentOutOfRangeException(nameof(versao), "A versão da fonte deve ser positiva.");
        }

        ArgumentNullException.ThrowIfNull(hash);
        ArgumentException.ThrowIfNullOrWhiteSpace(nomeOriginalSanitizado);
        var nomeSanitizado = nomeOriginalSanitizado.Trim();
        ValidarNomeSanitizado(nomeSanitizado);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeTypeValidado);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tamanhoBytes);

        return new FonteDocumental(
            id,
            versao,
            hash,
            nomeSanitizado,
            mimeTypeValidado.Trim().ToLowerInvariant(),
            tamanhoBytes);
    }

    private static void ValidarNomeSanitizado(string nome)
    {
        if (nome is "." or ".." || nome.IndexOfAny(CaracteresDeCaminho) >= 0 || nome.Any(char.IsControl))
        {
            throw new ArgumentException("O nome da fonte deve estar sanitizado e não pode conter caminho.", nameof(nome));
        }
    }
}
