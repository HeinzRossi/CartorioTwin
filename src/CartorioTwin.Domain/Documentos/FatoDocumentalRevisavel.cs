using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Documentos;

public sealed record FatoDocumentalRevisavel<TValor>
    where TValor : notnull
{
    private readonly IReadOnlyList<Pagina> _paginasAto;

    private FatoDocumentalRevisavel(
        FatoDocumentalId id,
        VersaoFatoDocumental versao,
        FatoDocumentalId? fatoAnteriorId,
        AtoRegistralId atoOrigemId,
        MatriculaId matriculaId,
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        IReadOnlyList<Pagina> paginasAto,
        string caminhoCampo,
        ValorOpcional<TValor> valorSugerido,
        IReadOnlyList<TrechoEvidencia> evidencias,
        ValorOpcional<TValor> valorFinal,
        EstadoRevisaoFato estado,
        SituacaoEvidenciaFato situacaoEvidencia,
        RevisaoFatoDocumental<TValor>? revisaoAplicada)
    {
        Id = id;
        Versao = versao;
        FatoAnteriorId = fatoAnteriorId;
        AtoOrigemId = atoOrigemId;
        MatriculaId = matriculaId;
        FonteId = fonteId;
        VersaoFonte = versaoFonte;
        _paginasAto = paginasAto;
        CaminhoCampo = caminhoCampo;
        ValorSugerido = valorSugerido;
        Evidencias = evidencias;
        ValorFinal = valorFinal;
        Estado = estado;
        SituacaoEvidencia = situacaoEvidencia;
        RevisaoAplicada = revisaoAplicada;
    }

    public FatoDocumentalId Id { get; }

    public VersaoFatoDocumental Versao { get; }

    public FatoDocumentalId? FatoAnteriorId { get; }

    public AtoRegistralId AtoOrigemId { get; }

    public MatriculaId MatriculaId { get; }

    public FonteDocumentalId FonteId { get; }

    public VersaoFonte VersaoFonte { get; }

    public string CaminhoCampo { get; }

    public ValorOpcional<TValor> ValorSugerido { get; }

    public IReadOnlyList<TrechoEvidencia> Evidencias { get; }

    public ValorOpcional<TValor> ValorFinal { get; }

    public EstadoRevisaoFato Estado { get; }

    public SituacaoEvidenciaFato SituacaoEvidencia { get; }

    public RevisaoFatoDocumental<TValor>? RevisaoAplicada { get; }

    public static FatoDocumentalRevisavel<TValor> CriarExtraido(
        FatoDocumentalId id,
        AtoRegistral atoOrigem,
        string caminhoCampo,
        TValor valorSugerido,
        IEnumerable<TrechoEvidencia> evidencias)
    {
        ArgumentNullException.ThrowIfNull(atoOrigem);
        var evidenciasCopiadas = ValidarEvidencias(
            atoOrigem.FonteId,
            atoOrigem.VersaoFonte,
            atoOrigem.Paginas,
            evidencias,
            exigirEvidencia: true);

        return CriarInicial(
            id,
            atoOrigem,
            caminhoCampo,
            ValorOpcional<TValor>.De(valorSugerido),
            evidenciasCopiadas,
            SituacaoEvidenciaFato.Localizada);
    }

    public static FatoDocumentalRevisavel<TValor> CriarNaoLocalizado(
        FatoDocumentalId id,
        AtoRegistral atoOrigem,
        string caminhoCampo)
    {
        ArgumentNullException.ThrowIfNull(atoOrigem);
        return CriarInicial(
            id,
            atoOrigem,
            caminhoCampo,
            ValorOpcional<TValor>.Ausente,
            Array.Empty<TrechoEvidencia>(),
            SituacaoEvidenciaFato.NaoLocalizada);
    }

    public static FatoDocumentalRevisavel<TValor> CriarNovaVersaoExtraida(
        FatoDocumentalId novoId,
        FatoDocumentalRevisavel<TValor> anterior,
        TValor valorSugerido,
        IEnumerable<TrechoEvidencia> evidencias)
    {
        ArgumentNullException.ThrowIfNull(anterior);
        ValidarNovaVersao(novoId, anterior);
        var evidenciasCopiadas = ValidarEvidencias(
            anterior.FonteId,
            anterior.VersaoFonte,
            anterior._paginasAto,
            evidencias,
            exigirEvidencia: true);

        return anterior.CriarSucessora(
            novoId,
            ValorOpcional<TValor>.De(valorSugerido),
            evidenciasCopiadas,
            SituacaoEvidenciaFato.Localizada);
    }

    public static FatoDocumentalRevisavel<TValor> CriarNovaVersaoNaoLocalizada(
        FatoDocumentalId novoId,
        FatoDocumentalRevisavel<TValor> anterior)
    {
        ArgumentNullException.ThrowIfNull(anterior);
        ValidarNovaVersao(novoId, anterior);
        return anterior.CriarSucessora(
            novoId,
            ValorOpcional<TValor>.Ausente,
            Array.Empty<TrechoEvidencia>(),
            SituacaoEvidenciaFato.NaoLocalizada);
    }

    public FatoDocumentalRevisavel<TValor> IniciarRevisao()
    {
        if (Estado != EstadoRevisaoFato.Extraido)
        {
            throw new InvalidOperationException("Somente fato extraído pode iniciar revisão.");
        }

        return Copiar(EstadoRevisaoFato.EmRevisao, SituacaoEvidencia, ValorOpcional<TValor>.Ausente, null);
    }

    public FatoDocumentalRevisavel<TValor> AplicarRevisao(RevisaoFatoDocumental<TValor> revisao)
    {
        ArgumentNullException.ThrowIfNull(revisao);
        if (Estado != EstadoRevisaoFato.EmRevisao)
        {
            throw new InvalidOperationException("Somente fato em revisão aceita decisão terminal.");
        }

        if (revisao.FatoId != Id || revisao.VersaoFato != Versao)
        {
            throw new ArgumentException("A revisão deve apontar para o fato e versão correntes.", nameof(revisao));
        }

        return revisao.Decisao switch
        {
            DecisaoRevisaoFato.Confirmar => AplicarConfirmacao(revisao),
            DecisaoRevisaoFato.Corrigir => AplicarCorrecao(revisao),
            DecisaoRevisaoFato.Contestar =>
                Copiar(EstadoRevisaoFato.Contestado, SituacaoEvidencia, ValorOpcional<TValor>.Ausente, revisao),
            DecisaoRevisaoFato.MarcarNaoLocalizado =>
                Copiar(EstadoRevisaoFato.Contestado, SituacaoEvidenciaFato.NaoLocalizada, ValorOpcional<TValor>.Ausente, revisao),
            DecisaoRevisaoFato.DeclararSemEvidencia => AplicarDeclaracaoSemEvidencia(revisao),
            DecisaoRevisaoFato.MarcarNaoSuportado =>
                Copiar(EstadoRevisaoFato.NaoSuportado, SituacaoEvidencia, ValorOpcional<TValor>.Ausente, revisao),
            _ => throw new ArgumentOutOfRangeException(nameof(revisao), "A decisão não pertence ao contrato."),
        };
    }

    private static FatoDocumentalRevisavel<TValor> CriarInicial(
        FatoDocumentalId id,
        AtoRegistral atoOrigem,
        string caminhoCampo,
        ValorOpcional<TValor> valorSugerido,
        IReadOnlyList<TrechoEvidencia> evidencias,
        SituacaoEvidenciaFato situacaoEvidencia)
    {
        if (id == default)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "O identificador do fato não pode ser vazio.");
        }

        return new FatoDocumentalRevisavel<TValor>(
            id,
            VersaoFatoDocumental.Criar(1),
            null,
            atoOrigem.Id,
            atoOrigem.MatriculaId,
            atoOrigem.FonteId,
            atoOrigem.VersaoFonte,
            Array.AsReadOnly(atoOrigem.Paginas.ToArray()),
            SanitizarCaminho(caminhoCampo),
            valorSugerido,
            evidencias,
            ValorOpcional<TValor>.Ausente,
            EstadoRevisaoFato.Extraido,
            situacaoEvidencia,
            null);
    }

    private static void ValidarNovaVersao(
        FatoDocumentalId novoId,
        FatoDocumentalRevisavel<TValor> anterior)
    {
        if (novoId == default || novoId == anterior.Id)
        {
            throw new ArgumentOutOfRangeException(nameof(novoId), "A nova versão exige identificador novo e não vazio.");
        }

        if (!anterior.EstadoEhTerminal())
        {
            throw new InvalidOperationException("Nova versão exige fato anterior em estado terminal.");
        }
    }

    private FatoDocumentalRevisavel<TValor> CriarSucessora(
        FatoDocumentalId novoId,
        ValorOpcional<TValor> valorSugerido,
        IReadOnlyList<TrechoEvidencia> evidencias,
        SituacaoEvidenciaFato situacaoEvidencia)
    {
        return new FatoDocumentalRevisavel<TValor>(
            novoId,
            Versao.Incrementar(),
            Id,
            AtoOrigemId,
            MatriculaId,
            FonteId,
            VersaoFonte,
            _paginasAto,
            CaminhoCampo,
            valorSugerido,
            evidencias,
            ValorOpcional<TValor>.Ausente,
            EstadoRevisaoFato.Extraido,
            situacaoEvidencia,
            null);
    }

    private FatoDocumentalRevisavel<TValor> AplicarConfirmacao(RevisaoFatoDocumental<TValor> revisao)
    {
        if (SituacaoEvidencia != SituacaoEvidenciaFato.Localizada ||
            !ValorSugerido.PossuiValor ||
            !revisao.ValorFinal.PossuiValor ||
            !EqualityComparer<TValor>.Default.Equals(ValorSugerido.Valor, revisao.ValorFinal.Valor))
        {
            throw new InvalidOperationException("Confirmação exige evidência localizada e valor final igual ao sugerido.");
        }

        return Copiar(EstadoRevisaoFato.Confirmado, SituacaoEvidencia, revisao.ValorFinal, revisao);
    }

    private FatoDocumentalRevisavel<TValor> AplicarCorrecao(RevisaoFatoDocumental<TValor> revisao)
    {
        if (SituacaoEvidencia != SituacaoEvidenciaFato.Localizada ||
            !ValorSugerido.PossuiValor ||
            !revisao.ValorFinal.PossuiValor ||
            EqualityComparer<TValor>.Default.Equals(ValorSugerido.Valor, revisao.ValorFinal.Valor))
        {
            throw new InvalidOperationException("Correção exige evidência localizada e valor final diferente do sugerido.");
        }

        return Copiar(EstadoRevisaoFato.Confirmado, SituacaoEvidencia, revisao.ValorFinal, revisao);
    }

    private FatoDocumentalRevisavel<TValor> AplicarDeclaracaoSemEvidencia(RevisaoFatoDocumental<TValor> revisao)
    {
        if (SituacaoEvidencia != SituacaoEvidenciaFato.NaoLocalizada ||
            Evidencias.Count != 0 ||
            !revisao.ValorFinal.PossuiValor)
        {
            throw new InvalidOperationException("Declaração sem evidência exige candidato sem fonte localizada.");
        }

        return Copiar(
            EstadoRevisaoFato.Confirmado,
            SituacaoEvidenciaFato.DeclaradaSemEvidencia,
            revisao.ValorFinal,
            revisao);
    }

    private FatoDocumentalRevisavel<TValor> Copiar(
        EstadoRevisaoFato estado,
        SituacaoEvidenciaFato situacaoEvidencia,
        ValorOpcional<TValor> valorFinal,
        RevisaoFatoDocumental<TValor>? revisaoAplicada)
    {
        return new FatoDocumentalRevisavel<TValor>(
            Id,
            Versao,
            FatoAnteriorId,
            AtoOrigemId,
            MatriculaId,
            FonteId,
            VersaoFonte,
            _paginasAto,
            CaminhoCampo,
            ValorSugerido,
            Evidencias,
            valorFinal,
            estado,
            situacaoEvidencia,
            revisaoAplicada);
    }

    private bool EstadoEhTerminal()
    {
        return Estado is EstadoRevisaoFato.Confirmado or
            EstadoRevisaoFato.Contestado or
            EstadoRevisaoFato.NaoSuportado;
    }

    private static IReadOnlyList<TrechoEvidencia> ValidarEvidencias(
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte,
        IReadOnlyList<Pagina> paginasAto,
        IEnumerable<TrechoEvidencia> evidencias,
        bool exigirEvidencia)
    {
        ArgumentNullException.ThrowIfNull(evidencias);
        var evidenciasCopiadas = evidencias.ToArray();
        if ((exigirEvidencia && evidenciasCopiadas.Length == 0) ||
            evidenciasCopiadas.Any(evidencia => evidencia is null))
        {
            throw new ArgumentException("O fato extraído exige ao menos uma evidência válida.", nameof(evidencias));
        }

        if (evidenciasCopiadas.Distinct().Count() != evidenciasCopiadas.Length)
        {
            throw new ArgumentException("O fato não pode repetir a mesma evidência.", nameof(evidencias));
        }

        if (evidenciasCopiadas.Any(evidencia =>
                evidencia.FonteId != fonteId ||
                evidencia.VersaoFonte != versaoFonte ||
                !paginasAto.Contains(evidencia.Pagina)))
        {
            throw new ArgumentException(
                "Toda evidência deve pertencer à fonte, versão e páginas do ato.",
                nameof(evidencias));
        }

        return Array.AsReadOnly(evidenciasCopiadas);
    }

    private static string SanitizarCaminho(string caminhoCampo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoCampo);
        var caminhoSanitizado = caminhoCampo.Trim();
        if (caminhoSanitizado.Any(char.IsControl))
        {
            throw new ArgumentException("O caminho do campo não pode conter caracteres de controle.", nameof(caminhoCampo));
        }

        return caminhoSanitizado;
    }
}
