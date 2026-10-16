using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Tests;

public sealed class FatoDocumentalRevisavelTests
{
    private static readonly FatoDocumentalId FatoId = FatoDocumentalId.Criar(
        Guid.Parse("61000000-0000-4000-8000-000000000001"));
    private static readonly RevisaoFatoDocumentalId RevisaoId = RevisaoFatoDocumentalId.Criar(
        Guid.Parse("62000000-0000-4000-8000-000000000001"));
    private static readonly UsuarioId UsuarioId = UsuarioId.Criar(
        Guid.Parse("63000000-0000-4000-8000-000000000001"));
    private static readonly AutorizacaoRevisaoId AutorizacaoId = AutorizacaoRevisaoId.Criar(
        Guid.Parse("64000000-0000-4000-8000-000000000001"));
    private static readonly DateTimeOffset InstanteUtc = new(2026, 10, 16, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Identificadores_RejeitamVazioEMantemValor()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FatoDocumentalId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => RevisaoFatoDocumentalId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => UsuarioId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => AutorizacaoRevisaoId.Criar(Guid.Empty));
        Assert.Equal(FatoId, FatoDocumentalId.Criar(FatoId.Valor));
        Assert.Equal(RevisaoId, RevisaoFatoDocumentalId.Criar(RevisaoId.Valor));
        Assert.Equal(UsuarioId, UsuarioId.Criar(UsuarioId.Valor));
        Assert.Equal(AutorizacaoId, AutorizacaoRevisaoId.Criar(AutorizacaoId.Valor));
    }

    [Fact]
    public void Versao_ExigePositivaEControlaOverflow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => VersaoFatoDocumental.Criar(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => VersaoFatoDocumental.Criar(-1));
        Assert.Equal(2, VersaoFatoDocumental.Criar(1).Incrementar().Numero);
        Assert.Throws<OverflowException>(() => VersaoFatoDocumental.Criar(long.MaxValue).Incrementar());
    }

    [Fact]
    public void ValorOpcional_DistingueAusenciaDeValor()
    {
        var ausente = ValorOpcional<string>.Ausente;
        var presente = ValorOpcional<string>.De("valor");

        Assert.False(ausente.PossuiValor);
        Assert.Throws<InvalidOperationException>(() => ausente.Valor);
        Assert.True(presente.PossuiValor);
        Assert.Equal("valor", presente.Valor);
        Assert.Throws<ArgumentNullException>(() => ValorOpcional<string>.De(null!));
    }

    [Fact]
    public void FatoExtraido_DerivaContextoSanitizaCampoECopiaEvidencias()
    {
        var contexto = CriarContexto();
        var evidencias = new[] { contexto.Evidencia };
        var fato = FatoDocumentalRevisavel<string>.CriarExtraido(
            FatoId,
            contexto.Ato,
            " holder.name ",
            "Pessoa Alfa",
            evidencias);

        evidencias[0] = CriarEvidencia(contexto.Fonte, Pagina.Criar(1), "Outro texto");

        Assert.Equal(EstadoRevisaoFato.Extraido, fato.Estado);
        Assert.Equal(SituacaoEvidenciaFato.Localizada, fato.SituacaoEvidencia);
        Assert.Equal("holder.name", fato.CaminhoCampo);
        Assert.Equal("Pessoa Alfa", fato.ValorSugerido.Valor);
        Assert.Equal(contexto.Evidencia, fato.Evidencias.Single());
        Assert.Equal(contexto.Ato.Id, fato.AtoOrigemId);
        Assert.Equal(contexto.Ato.MatriculaId, fato.MatriculaId);
        Assert.Equal(contexto.Fonte.Id, fato.FonteId);
        Assert.Equal(contexto.Fonte.Versao, fato.VersaoFonte);
        Assert.Equal(1, fato.Versao.Numero);
        Assert.Null(fato.FatoAnteriorId);
        Assert.False(fato.ValorFinal.PossuiValor);
        Assert.Null(fato.RevisaoAplicada);
    }

    [Fact]
    public void FatoExtraido_RejeitaCampoOuEvidenciaInvalida()
    {
        var contexto = CriarContexto();
        var outraFonte = CriarFonte(
            FonteDocumentalId.Criar(Guid.Parse("65000000-0000-4000-8000-000000000002")),
            VersaoFonte.Criar(1));

        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(FatoId, contexto.Ato, " ", "valor", [contexto.Evidencia]));
        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(FatoId, contexto.Ato, "a\nb", "valor", [contexto.Evidencia]));
        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(FatoId, contexto.Ato, "field", "valor", []));
        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(
                FatoId,
                contexto.Ato,
                "field",
                "valor",
                [contexto.Evidencia, contexto.Evidencia]));
        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(
                FatoId,
                contexto.Ato,
                "field",
                "valor",
                [CriarEvidencia(outraFonte, Pagina.Criar(1), "Outra fonte")]));
        Assert.Throws<ArgumentException>(
            () => FatoDocumentalRevisavel<string>.CriarExtraido(
                FatoId,
                contexto.Ato,
                "field",
                "valor",
                [CriarEvidencia(contexto.Fonte, Pagina.Criar(2), "Outra página")]));
    }

    [Fact]
    public void IniciarRevisao_NaoMutaCandidatoOriginal()
    {
        var extraido = CriarFatoExtraido();

        var emRevisao = extraido.IniciarRevisao();

        Assert.Equal(EstadoRevisaoFato.Extraido, extraido.Estado);
        Assert.Equal(EstadoRevisaoFato.EmRevisao, emRevisao.Estado);
        Assert.Throws<InvalidOperationException>(() => emRevisao.IniciarRevisao());
    }

    [Fact]
    public void Revisao_ExigeIdentidadesAutorizacaoVersaoEUtc()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RevisaoFatoDocumental<string>.CriarComValorFinal(
                default,
                FatoId,
                VersaoFatoDocumental.Criar(1),
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                null,
                UsuarioId,
                AutorizacaoId,
                InstanteUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RevisaoFatoDocumental<string>.CriarComValorFinal(
                RevisaoId,
                default,
                VersaoFatoDocumental.Criar(1),
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                null,
                UsuarioId,
                AutorizacaoId,
                InstanteUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RevisaoFatoDocumental<string>.CriarComValorFinal(
                RevisaoId,
                FatoId,
                default,
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                null,
                UsuarioId,
                AutorizacaoId,
                InstanteUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RevisaoFatoDocumental<string>.CriarComValorFinal(
                RevisaoId,
                FatoId,
                VersaoFatoDocumental.Criar(1),
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                null,
                default,
                AutorizacaoId,
                InstanteUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RevisaoFatoDocumental<string>.CriarComValorFinal(
                RevisaoId,
                FatoId,
                VersaoFatoDocumental.Criar(1),
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                null,
                UsuarioId,
                default,
                InstanteUtc));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CriarRevisaoComValor(
                DecisaoRevisaoFato.Confirmar,
                "Pessoa Alfa",
                emUtc: new DateTimeOffset(2026, 10, 16, 9, 0, 0, TimeSpan.FromHours(-3))));
    }

    [Fact]
    public void Revisao_SeparaDecisoesComESemValorFinal()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CriarRevisaoComValor(DecisaoRevisaoFato.Contestar, "valor", justificativa: "Motivo"));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CriarRevisaoSemValor(DecisaoRevisaoFato.Confirmar, justificativa: null));
        Assert.Throws<ArgumentException>(
            () => CriarRevisaoComValor(DecisaoRevisaoFato.Corrigir, "valor", justificativa: " "));
        Assert.Throws<ArgumentException>(
            () => CriarRevisaoSemValor(DecisaoRevisaoFato.Contestar, justificativa: "inválida\n"));

        var confirmacao = CriarRevisaoComValor(DecisaoRevisaoFato.Confirmar, "Pessoa Alfa");
        Assert.True(confirmacao.ValorFinal.PossuiValor);
        Assert.Null(confirmacao.Justificativa);
        Assert.Equal(UsuarioId, confirmacao.UsuarioId);
        Assert.Equal(AutorizacaoId, confirmacao.AutorizacaoId);
        Assert.Equal(InstanteUtc, confirmacao.EmUtc);
    }

    [Fact]
    public void Confirmacao_PromoveSomenteValorSugeridoComEvidencia()
    {
        var emRevisao = CriarFatoExtraido().IniciarRevisao();
        var confirmacao = CriarRevisaoComValor(DecisaoRevisaoFato.Confirmar, "Pessoa Alfa");

        var confirmado = emRevisao.AplicarRevisao(confirmacao);

        Assert.Equal(EstadoRevisaoFato.Confirmado, confirmado.Estado);
        Assert.Equal(SituacaoEvidenciaFato.Localizada, confirmado.SituacaoEvidencia);
        Assert.Equal("Pessoa Alfa", confirmado.ValorFinal.Valor);
        Assert.Equal(confirmacao, confirmado.RevisaoAplicada);
        Assert.Throws<InvalidOperationException>(
            () => emRevisao.AplicarRevisao(CriarRevisaoComValor(DecisaoRevisaoFato.Confirmar, "Pessoa Beta")));
    }

    [Fact]
    public void Correcao_ExigeValorDiferenteEJustificativa()
    {
        var emRevisao = CriarFatoExtraido().IniciarRevisao();
        var correcao = CriarRevisaoComValor(DecisaoRevisaoFato.Corrigir, "Pessoa Beta", "OCR corrigido");

        var confirmado = emRevisao.AplicarRevisao(correcao);

        Assert.Equal(EstadoRevisaoFato.Confirmado, confirmado.Estado);
        Assert.Equal("Pessoa Alfa", confirmado.ValorSugerido.Valor);
        Assert.Equal("Pessoa Beta", confirmado.ValorFinal.Valor);
        Assert.Equal("OCR corrigido", confirmado.RevisaoAplicada!.Justificativa);
        Assert.Throws<InvalidOperationException>(
            () => emRevisao.AplicarRevisao(
                CriarRevisaoComValor(DecisaoRevisaoFato.Corrigir, "Pessoa Alfa", "Sem alteração")));
    }

    [Fact]
    public void Contestacao_PreservaCandidatoSemValorFinal()
    {
        var contestado = CriarFatoExtraido()
            .IniciarRevisao()
            .AplicarRevisao(CriarRevisaoSemValor(DecisaoRevisaoFato.Contestar, "Identidade ambígua"));

        Assert.Equal(EstadoRevisaoFato.Contestado, contestado.Estado);
        Assert.Equal(SituacaoEvidenciaFato.Localizada, contestado.SituacaoEvidencia);
        Assert.Equal("Pessoa Alfa", contestado.ValorSugerido.Valor);
        Assert.False(contestado.ValorFinal.PossuiValor);
    }

    [Fact]
    public void NaoLocalizado_ContestaEFixaProvenienciaSeparada()
    {
        var fato = CriarFatoExtraido()
            .IniciarRevisao()
            .AplicarRevisao(CriarRevisaoSemValor(DecisaoRevisaoFato.MarcarNaoLocalizado, "Trecho não sustenta campo"));

        Assert.Equal(EstadoRevisaoFato.Contestado, fato.Estado);
        Assert.Equal(SituacaoEvidenciaFato.NaoLocalizada, fato.SituacaoEvidencia);
        Assert.False(fato.ValorFinal.PossuiValor);
    }

    [Fact]
    public void DeclaracaoSemEvidencia_ConfirmaDeclaracaoSemFabricarFonte()
    {
        var contexto = CriarContexto();
        var candidato = FatoDocumentalRevisavel<string>.CriarNaoLocalizado(FatoId, contexto.Ato, "holder.name");
        var revisao = CriarRevisaoComValor(
            DecisaoRevisaoFato.DeclararSemEvidencia,
            "Pessoa declarada",
            "Informação manual para contingência");

        var declarado = candidato.IniciarRevisao().AplicarRevisao(revisao);

        Assert.Equal(EstadoRevisaoFato.Confirmado, declarado.Estado);
        Assert.Equal(SituacaoEvidenciaFato.DeclaradaSemEvidencia, declarado.SituacaoEvidencia);
        Assert.Empty(declarado.Evidencias);
        Assert.False(declarado.ValorSugerido.PossuiValor);
        Assert.Equal("Pessoa declarada", declarado.ValorFinal.Valor);

        Assert.Throws<InvalidOperationException>(
            () => CriarFatoExtraido().IniciarRevisao().AplicarRevisao(revisao));
    }

    [Fact]
    public void NaoSuportado_EncerraSemAplicarValorFinal()
    {
        var fato = CriarFatoExtraido()
            .IniciarRevisao()
            .AplicarRevisao(CriarRevisaoSemValor(DecisaoRevisaoFato.MarcarNaoSuportado, "Sem regra técnica"));

        Assert.Equal(EstadoRevisaoFato.NaoSuportado, fato.Estado);
        Assert.False(fato.ValorFinal.PossuiValor);
    }

    [Fact]
    public void AplicarRevisao_RejeitaSaltoAlvoDiferenteEReaplicacao()
    {
        var extraido = CriarFatoExtraido();
        var emRevisao = extraido.IniciarRevisao();
        var confirmacao = CriarRevisaoComValor(DecisaoRevisaoFato.Confirmar, "Pessoa Alfa");

        Assert.Throws<InvalidOperationException>(() => extraido.AplicarRevisao(confirmacao));
        Assert.Throws<ArgumentException>(
            () => emRevisao.AplicarRevisao(
                CriarRevisaoComValor(
                    DecisaoRevisaoFato.Confirmar,
                    "Pessoa Alfa",
                    fatoId: FatoDocumentalId.Criar(Guid.Parse("61000000-0000-4000-8000-000000000002")))));
        Assert.Throws<ArgumentException>(
            () => emRevisao.AplicarRevisao(
                CriarRevisaoComValor(
                    DecisaoRevisaoFato.Confirmar,
                    "Pessoa Alfa",
                    versao: VersaoFatoDocumental.Criar(2))));

        var confirmado = emRevisao.AplicarRevisao(confirmacao);
        Assert.Throws<InvalidOperationException>(() => confirmado.AplicarRevisao(confirmacao));
        Assert.Throws<InvalidOperationException>(() => confirmado.IniciarRevisao());
    }

    [Fact]
    public void NovaVersao_ExigeAnteriorTerminalEPreservaHistorico()
    {
        var contexto = CriarContexto();
        var extraido = CriarFatoExtraido(contexto);
        Assert.Throws<InvalidOperationException>(
            () => FatoDocumentalRevisavel<string>.CriarNovaVersaoExtraida(
                FatoDocumentalId.Criar(Guid.Parse("61000000-0000-4000-8000-000000000003")),
                extraido,
                "Pessoa Beta",
                [contexto.Evidencia]));

        var confirmado = extraido
            .IniciarRevisao()
            .AplicarRevisao(CriarRevisaoComValor(DecisaoRevisaoFato.Confirmar, "Pessoa Alfa"));
        var novoId = FatoDocumentalId.Criar(Guid.Parse("61000000-0000-4000-8000-000000000003"));
        var novaVersao = FatoDocumentalRevisavel<string>.CriarNovaVersaoExtraida(
            novoId,
            confirmado,
            "Pessoa Beta",
            [contexto.Evidencia]);

        Assert.Equal(EstadoRevisaoFato.Confirmado, confirmado.Estado);
        Assert.Equal("Pessoa Alfa", confirmado.ValorFinal.Valor);
        Assert.Equal(EstadoRevisaoFato.Extraido, novaVersao.Estado);
        Assert.Equal(2, novaVersao.Versao.Numero);
        Assert.Equal(confirmado.Id, novaVersao.FatoAnteriorId);
        Assert.Equal("Pessoa Beta", novaVersao.ValorSugerido.Valor);
        Assert.False(novaVersao.ValorFinal.PossuiValor);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => FatoDocumentalRevisavel<string>.CriarNovaVersaoNaoLocalizada(confirmado.Id, confirmado));
    }

    private static FatoDocumentalRevisavel<string> CriarFatoExtraido(Contexto? contexto = null)
    {
        contexto ??= CriarContexto();
        return FatoDocumentalRevisavel<string>.CriarExtraido(
            FatoId,
            contexto.Ato,
            "holder.name",
            "Pessoa Alfa",
            [contexto.Evidencia]);
    }

    private static RevisaoFatoDocumental<string> CriarRevisaoComValor(
        DecisaoRevisaoFato decisao,
        string valorFinal,
        string? justificativa = null,
        RevisaoFatoDocumentalId? id = null,
        FatoDocumentalId? fatoId = null,
        VersaoFatoDocumental? versao = null,
        UsuarioId? usuarioId = null,
        AutorizacaoRevisaoId? autorizacaoId = null,
        DateTimeOffset? emUtc = null)
    {
        return RevisaoFatoDocumental<string>.CriarComValorFinal(
            id ?? RevisaoId,
            fatoId ?? FatoId,
            versao ?? VersaoFatoDocumental.Criar(1),
            decisao,
            valorFinal,
            justificativa,
            usuarioId ?? UsuarioId,
            autorizacaoId ?? AutorizacaoId,
            emUtc ?? InstanteUtc);
    }

    private static RevisaoFatoDocumental<string> CriarRevisaoSemValor(
        DecisaoRevisaoFato decisao,
        string? justificativa)
    {
        return RevisaoFatoDocumental<string>.CriarSemValorFinal(
            RevisaoId,
            FatoId,
            VersaoFatoDocumental.Criar(1),
            decisao,
            justificativa,
            UsuarioId,
            AutorizacaoId,
            InstanteUtc);
    }

    private static Contexto CriarContexto()
    {
        const string texto = "Pessoa Alfa consta no ato sintético.";
        var fonte = CriarFonte(
            FonteDocumentalId.Criar(Guid.Parse("65000000-0000-4000-8000-000000000001")),
            VersaoFonte.Criar(1));
        var pagina = Pagina.Criar(1);
        var evidencia = TrechoEvidencia.Criar(fonte, pagina, texto, 0, 11);
        var ato = AtoRegistral.Criar(
            AtoRegistralId.Criar(Guid.Parse("66000000-0000-4000-8000-000000000001")),
            MatriculaId.Criar(Guid.Parse("67000000-0000-4000-8000-000000000001")),
            fonte.Id,
            fonte.Versao,
            IdentificadorAtoRegistral.CriarRegistro(1),
            OrdemAtoRegistral.Criar(1),
            DatasDocumentaisAto.Nenhuma,
            [pagina],
            [],
            []);

        return new Contexto(fonte, ato, evidencia);
    }

    private static FonteDocumental CriarFonte(FonteDocumentalId id, VersaoFonte versao)
    {
        return FonteDocumental.Criar(
            id,
            versao,
            HashSha256.CalcularUtf8($"fonte-{id}-{versao}"),
            "revisao-sintetica.pdf",
            "application/pdf",
            1024);
    }

    private static TrechoEvidencia CriarEvidencia(FonteDocumental fonte, Pagina pagina, string texto)
    {
        return TrechoEvidencia.Criar(fonte, pagina, texto, 0, texto.Length);
    }

    private sealed record Contexto(
        FonteDocumental Fonte,
        AtoRegistral Ato,
        TrechoEvidencia Evidencia);
}
