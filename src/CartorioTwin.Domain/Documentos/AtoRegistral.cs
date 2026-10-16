using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public sealed record AtoRegistral
{
    private AtoRegistral(
        AtoRegistralId id,
        MatriculaId matriculaId,
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        IdentificadorAtoRegistral identificador,
        OrdemAtoRegistral ordemNaFonte,
        DatasDocumentaisAto datas,
        IReadOnlyList<Pagina> paginas,
        IReadOnlyList<ParteAtoRegistral> partes,
        IReadOnlyList<RelacaoAtoRegistral> relacoes)
    {
        Id = id;
        MatriculaId = matriculaId;
        FonteId = fonteId;
        VersaoFonte = versaoFonte;
        Identificador = identificador;
        OrdemNaFonte = ordemNaFonte;
        Datas = datas;
        Paginas = paginas;
        Partes = partes;
        Relacoes = relacoes;
    }

    public AtoRegistralId Id { get; }

    public MatriculaId MatriculaId { get; }

    public FonteDocumentalId FonteId { get; }

    public VersaoFonte VersaoFonte { get; }

    public IdentificadorAtoRegistral Identificador { get; }

    public OrdemAtoRegistral OrdemNaFonte { get; }

    public DatasDocumentaisAto Datas { get; }

    public IReadOnlyList<Pagina> Paginas { get; }

    public IReadOnlyList<ParteAtoRegistral> Partes { get; }

    public IReadOnlyList<RelacaoAtoRegistral> Relacoes { get; }

    public static AtoRegistral Criar(
        AtoRegistralId id,
        MatriculaId matriculaId,
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        IdentificadorAtoRegistral identificador,
        OrdemAtoRegistral ordemNaFonte,
        DatasDocumentaisAto datas,
        IEnumerable<Pagina> paginas,
        IEnumerable<ParteAtoRegistral> partes,
        IEnumerable<RelacaoAtoRegistral> relacoes)
    {
        ValidarIdentidades(id, matriculaId, fonteId, versaoFonte, identificador, ordemNaFonte);
        ArgumentNullException.ThrowIfNull(datas);
        ArgumentNullException.ThrowIfNull(paginas);
        ArgumentNullException.ThrowIfNull(partes);
        ArgumentNullException.ThrowIfNull(relacoes);

        var paginasCopiadas = paginas.ToArray();
        if (paginasCopiadas.Length == 0 || paginasCopiadas.Any(pagina => pagina == default))
        {
            throw new ArgumentException("O ato deve possuir ao menos uma página válida.", nameof(paginas));
        }

        if (paginasCopiadas.Distinct().Count() != paginasCopiadas.Length)
        {
            throw new ArgumentException("O ato não pode repetir páginas.", nameof(paginas));
        }

        var partesCopiadas = partes.ToArray();
        if (partesCopiadas.Any(parte => parte is null))
        {
            throw new ArgumentException("A coleção de partes não pode conter valor nulo.", nameof(partes));
        }

        var relacoesCopiadas = relacoes.ToArray();
        if (relacoesCopiadas.Any(relacao => relacao.AtoRelacionadoId == default))
        {
            throw new ArgumentException("A coleção de relações não pode conter valor inválido.", nameof(relacoes));
        }

        if (relacoesCopiadas.Any(relacao => relacao.AtoRelacionadoId == id))
        {
            throw new ArgumentException("Um ato não pode se relacionar consigo mesmo.", nameof(relacoes));
        }

        if (relacoesCopiadas.Distinct().Count() != relacoesCopiadas.Length)
        {
            throw new ArgumentException("O ato não pode repetir a mesma relação.", nameof(relacoes));
        }

        return new AtoRegistral(
            id,
            matriculaId,
            fonteId,
            versaoFonte,
            identificador,
            ordemNaFonte,
            datas,
            Array.AsReadOnly(paginasCopiadas),
            Array.AsReadOnly(partesCopiadas),
            Array.AsReadOnly(relacoesCopiadas));
    }

    private static void ValidarIdentidades(
        AtoRegistralId id,
        MatriculaId matriculaId,
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        IdentificadorAtoRegistral identificador,
        OrdemAtoRegistral ordemNaFonte)
    {
        if (id == default)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador do ato não pode ser vazio.");
        }

        if (matriculaId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(matriculaId), "A matrícula do ato não pode ser vazia.");
        }

        if (fonteId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(fonteId), "A fonte do ato não pode ser vazia.");
        }

        if (versaoFonte == default)
        {
            throw new ArgumentOutOfRangeException(nameof(versaoFonte), "A versão da fonte deve ser positiva.");
        }

        if (string.IsNullOrWhiteSpace(identificador.TextoOriginal))
        {
            throw new ArgumentOutOfRangeException(nameof(identificador), "O identificador documental deve ser explícito.");
        }

        if (ordemNaFonte == default)
        {
            throw new ArgumentOutOfRangeException(nameof(ordemNaFonte), "A ordem documental deve ser positiva.");
        }
    }
}
