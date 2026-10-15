namespace CartorioTwin.Domain.Documentos;

public sealed record TrechoEvidencia
{
    private TrechoEvidencia(
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        HashSha256 hashFonte,
        Pagina pagina,
        HashSha256 hashTextoPagina,
        int inicioUtf16,
        int fimUtf16,
        string trecho)
    {
        FonteId = fonteId;
        VersaoFonte = versaoFonte;
        HashFonte = hashFonte;
        Pagina = pagina;
        HashTextoPagina = hashTextoPagina;
        InicioUtf16 = inicioUtf16;
        FimUtf16 = fimUtf16;
        Trecho = trecho;
    }

    public FonteDocumentalId FonteId { get; }

    public VersaoFonte VersaoFonte { get; }

    public HashSha256 HashFonte { get; }

    public Pagina Pagina { get; }

    public HashSha256 HashTextoPagina { get; }

    public int InicioUtf16 { get; }

    public int FimUtf16 { get; }

    public string Trecho { get; }

    public static TrechoEvidencia Criar(
        FonteDocumental fonte,
        Pagina pagina,
        string textoPagina,
        int inicioUtf16,
        int fimUtf16)
    {
        ArgumentNullException.ThrowIfNull(fonte);
        ArgumentNullException.ThrowIfNull(textoPagina);

        if (pagina == default)
        {
            throw new ArgumentOutOfRangeException(nameof(pagina), "A página deve iniciar em 1.");
        }

        if (inicioUtf16 < 0 || fimUtf16 > textoPagina.Length || inicioUtf16 >= fimUtf16)
        {
            throw new ArgumentOutOfRangeException(
                nameof(inicioUtf16),
                "O intervalo UTF-16 deve ser não vazio e pertencer ao texto da página.");
        }

        if (DivideParSubstituto(textoPagina, inicioUtf16) || DivideParSubstituto(textoPagina, fimUtf16))
        {
            throw new ArgumentException("O intervalo UTF-16 não pode dividir um par substituto.");
        }

        return new TrechoEvidencia(
            fonte.Id,
            fonte.Versao,
            fonte.Hash,
            pagina,
            HashSha256.CalcularUtf8(textoPagina),
            inicioUtf16,
            fimUtf16,
            textoPagina[inicioUtf16..fimUtf16]);
    }

    private static bool DivideParSubstituto(string texto, int indice)
    {
        return indice > 0 &&
            indice < texto.Length &&
            char.IsHighSurrogate(texto[indice - 1]) &&
            char.IsLowSurrogate(texto[indice]);
    }
}
