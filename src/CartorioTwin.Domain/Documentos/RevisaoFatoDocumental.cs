using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public sealed record RevisaoFatoDocumental<TValor>
    where TValor : notnull
{
    private RevisaoFatoDocumental(
        RevisaoFatoDocumentalId id,
        FatoDocumentalId fatoId,
        VersaoFatoDocumental versaoFato,
        DecisaoRevisaoFato decisao,
        ValorOpcional<TValor> valorFinal,
        string? justificativa,
        UsuarioId usuarioId,
        AutorizacaoRevisaoId autorizacaoId,
        DateTimeOffset emUtc)
    {
        Id = id;
        FatoId = fatoId;
        VersaoFato = versaoFato;
        Decisao = decisao;
        ValorFinal = valorFinal;
        Justificativa = justificativa;
        UsuarioId = usuarioId;
        AutorizacaoId = autorizacaoId;
        EmUtc = emUtc;
    }

    public RevisaoFatoDocumentalId Id { get; }

    public FatoDocumentalId FatoId { get; }

    public VersaoFatoDocumental VersaoFato { get; }

    public DecisaoRevisaoFato Decisao { get; }

    public ValorOpcional<TValor> ValorFinal { get; }

    public string? Justificativa { get; }

    public UsuarioId UsuarioId { get; }

    public AutorizacaoRevisaoId AutorizacaoId { get; }

    public DateTimeOffset EmUtc { get; }

    public static RevisaoFatoDocumental<TValor> CriarComValorFinal(
        RevisaoFatoDocumentalId id,
        FatoDocumentalId fatoId,
        VersaoFatoDocumental versaoFato,
        DecisaoRevisaoFato decisao,
        TValor valorFinal,
        string? justificativa,
        UsuarioId usuarioId,
        AutorizacaoRevisaoId autorizacaoId,
        DateTimeOffset emUtc)
    {
        if (decisao is not DecisaoRevisaoFato.Confirmar and
            not DecisaoRevisaoFato.Corrigir and
            not DecisaoRevisaoFato.DeclararSemEvidencia)
        {
            throw new ArgumentOutOfRangeException(nameof(decisao), "A decisão informada não aceita valor final.");
        }

        return Criar(
            id,
            fatoId,
            versaoFato,
            decisao,
            ValorOpcional<TValor>.De(valorFinal),
            justificativa,
            usuarioId,
            autorizacaoId,
            emUtc);
    }

    public static RevisaoFatoDocumental<TValor> CriarSemValorFinal(
        RevisaoFatoDocumentalId id,
        FatoDocumentalId fatoId,
        VersaoFatoDocumental versaoFato,
        DecisaoRevisaoFato decisao,
        string? justificativa,
        UsuarioId usuarioId,
        AutorizacaoRevisaoId autorizacaoId,
        DateTimeOffset emUtc)
    {
        if (decisao is not DecisaoRevisaoFato.Contestar and
            not DecisaoRevisaoFato.MarcarNaoLocalizado and
            not DecisaoRevisaoFato.MarcarNaoSuportado)
        {
            throw new ArgumentOutOfRangeException(nameof(decisao), "A decisão informada exige valor final.");
        }

        return Criar(
            id,
            fatoId,
            versaoFato,
            decisao,
            ValorOpcional<TValor>.Ausente,
            justificativa,
            usuarioId,
            autorizacaoId,
            emUtc);
    }

    private static RevisaoFatoDocumental<TValor> Criar(
        RevisaoFatoDocumentalId id,
        FatoDocumentalId fatoId,
        VersaoFatoDocumental versaoFato,
        DecisaoRevisaoFato decisao,
        ValorOpcional<TValor> valorFinal,
        string? justificativa,
        UsuarioId usuarioId,
        AutorizacaoRevisaoId autorizacaoId,
        DateTimeOffset emUtc)
    {
        if (id == default)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador da revisão não pode ser vazio.");
        }

        if (fatoId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(fatoId), "O fato revisado não pode ser vazio.");
        }

        if (versaoFato == default)
        {
            throw new ArgumentOutOfRangeException(nameof(versaoFato), "A versão revisada deve ser positiva.");
        }

        if (usuarioId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(usuarioId), "O revisor não pode ser vazio.");
        }

        if (autorizacaoId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(autorizacaoId), "A revisão exige referência de autorização.");
        }

        if (emUtc == default || emUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(emUtc), "O instante da revisão deve ser UTC explícito.");
        }

        var justificativaSanitizada = SanitizarJustificativa(justificativa);
        if (decisao != DecisaoRevisaoFato.Confirmar && justificativaSanitizada is null)
        {
            throw new ArgumentException("A decisão exige justificativa.", nameof(justificativa));
        }

        return new RevisaoFatoDocumental<TValor>(
            id,
            fatoId,
            versaoFato,
            decisao,
            valorFinal,
            justificativaSanitizada,
            usuarioId,
            autorizacaoId,
            emUtc);
    }

    private static string? SanitizarJustificativa(string? justificativa)
    {
        if (string.IsNullOrWhiteSpace(justificativa))
        {
            return null;
        }

        if (justificativa.Any(char.IsControl))
        {
            throw new ArgumentException("A justificativa não pode conter caracteres de controle.", nameof(justificativa));
        }

        return justificativa.Trim();
    }
}
