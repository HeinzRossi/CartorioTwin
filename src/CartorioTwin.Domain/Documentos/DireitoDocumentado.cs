using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public sealed record DireitoDocumentado
{
    private DireitoDocumentado(
        DireitoDocumentadoId id,
        AtoRegistral atoOrigem,
        TipoDireitoDocumentado tipo,
        ExtensaoDireitoDocumentado extensao,
        ConhecimentoEfeitoDireito conhecimentoEfeito,
        DateOnly? inicioDocumentado,
        DateOnly? fimDocumentado,
        IReadOnlyList<PessoaId> titulares,
        IReadOnlyList<TrechoEvidencia> evidencias,
        IReadOnlyList<ChaveTitularidadeDocumentada> chavesTitularidade)
    {
        Id = id;
        AtoOrigemId = atoOrigem.Id;
        MatriculaId = atoOrigem.MatriculaId;
        FonteId = atoOrigem.FonteId;
        VersaoFonte = atoOrigem.VersaoFonte;
        Tipo = tipo;
        Extensao = extensao;
        ConhecimentoEfeito = conhecimentoEfeito;
        InicioDocumentado = inicioDocumentado;
        FimDocumentado = fimDocumentado;
        Titulares = titulares;
        Evidencias = evidencias;
        ChavesTitularidade = chavesTitularidade;
    }

    public DireitoDocumentadoId Id { get; }

    public AtoRegistralId AtoOrigemId { get; }

    public MatriculaId MatriculaId { get; }

    public FonteDocumentalId FonteId { get; }

    public VersaoFonte VersaoFonte { get; }

    public TipoDireitoDocumentado Tipo { get; }

    public ExtensaoDireitoDocumentado Extensao { get; }

    public ConhecimentoEfeitoDireito ConhecimentoEfeito { get; }

    public DateOnly? InicioDocumentado { get; }

    public DateOnly? FimDocumentado { get; }

    public IReadOnlyList<PessoaId> Titulares { get; }

    public IReadOnlyList<TrechoEvidencia> Evidencias { get; }

    public IReadOnlyList<ChaveTitularidadeDocumentada> ChavesTitularidade { get; }

    public static DireitoDocumentado Criar(
        DireitoDocumentadoId id,
        AtoRegistral atoOrigem,
        TipoDireitoDocumentado tipo,
        ExtensaoDireitoDocumentado extensao,
        ConhecimentoEfeitoDireito conhecimentoEfeito,
        DateOnly? inicioDocumentado,
        DateOnly? fimDocumentado,
        IEnumerable<PessoaId> titulares,
        IEnumerable<TrechoEvidencia> evidencias)
    {
        if (id == default)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador do direito não pode ser vazio.");
        }

        ArgumentNullException.ThrowIfNull(atoOrigem);
        ValidarEnum(tipo, nameof(tipo));
        ValidarEnum(conhecimentoEfeito, nameof(conhecimentoEfeito));
        ArgumentNullException.ThrowIfNull(titulares);
        ArgumentNullException.ThrowIfNull(evidencias);

        var titularesCopiados = titulares.ToArray();
        if (titularesCopiados.Any(titular => titular == default))
        {
            throw new ArgumentException("Titulares não podem conter identidade vazia.", nameof(titulares));
        }

        if (titularesCopiados.Distinct().Count() != titularesCopiados.Length)
        {
            throw new ArgumentException("O mesmo titular não pode aparecer repetido no direito.", nameof(titulares));
        }

        var evidenciasCopiadas = evidencias.ToArray();
        if (evidenciasCopiadas.Length == 0 || evidenciasCopiadas.Any(evidencia => evidencia is null))
        {
            throw new ArgumentException("O direito deve possuir ao menos uma evidência válida.", nameof(evidencias));
        }

        if (evidenciasCopiadas.Distinct().Count() != evidenciasCopiadas.Length)
        {
            throw new ArgumentException("O direito não pode repetir a mesma evidência.", nameof(evidencias));
        }

        if (evidenciasCopiadas.Any(evidencia =>
                evidencia.FonteId != atoOrigem.FonteId ||
                evidencia.VersaoFonte != atoOrigem.VersaoFonte ||
                !atoOrigem.Paginas.Contains(evidencia.Pagina)))
        {
            throw new ArgumentException(
                "Toda evidência deve pertencer à fonte, versão e páginas do ato de origem.",
                nameof(evidencias));
        }

        var chaves = titularesCopiados
            .Select(titular => ChaveTitularidadeDocumentada.Criar(atoOrigem.MatriculaId, titular, tipo))
            .ToArray();

        return new DireitoDocumentado(
            id,
            atoOrigem,
            tipo,
            extensao,
            conhecimentoEfeito,
            inicioDocumentado,
            fimDocumentado,
            Array.AsReadOnly(titularesCopiados),
            Array.AsReadOnly(evidenciasCopiadas),
            Array.AsReadOnly(chaves));
    }

    private static void ValidarEnum<TEnum>(TEnum valor, string nomeParametro)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(valor))
        {
            throw new ArgumentOutOfRangeException(nomeParametro, "O valor informado não pertence ao contrato.");
        }
    }
}
