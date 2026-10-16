using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;
using CartorioTwin.Domain.Shared;

namespace CartorioTwin.Domain.Tests;

public sealed class DireitoDocumentadoTests
{
    private static readonly DireitoDocumentadoId DireitoId = DireitoDocumentadoId.Criar(
        Guid.Parse("10000000-0000-4000-8000-000000000001"));
    private static readonly AtoRegistralId AtoId = AtoRegistralId.Criar(
        Guid.Parse("20000000-0000-4000-8000-000000000001"));
    private static readonly MatriculaId MatriculaId = MatriculaId.Criar(
        Guid.Parse("30000000-0000-4000-8000-000000000001"));
    private static readonly FonteDocumentalId FonteId = FonteDocumentalId.Criar(
        Guid.Parse("40000000-0000-4000-8000-000000000001"));
    private static readonly PessoaId PessoaId = PessoaId.Criar(
        Guid.Parse("50000000-0000-4000-8000-000000000001"));

    [Fact]
    public void DireitoDocumentadoId_RejeitaVazioEMantemIgualdade()
    {
        var valor = Guid.Parse("10000000-0000-4000-8000-000000000002");

        Assert.Throws<ArgumentOutOfRangeException>(() => DireitoDocumentadoId.Criar(Guid.Empty));
        Assert.Equal(DireitoDocumentadoId.Criar(valor), DireitoDocumentadoId.Criar(valor));
    }

    [Theory]
    [InlineData(TipoDireitoDocumentado.NaoIdentificado)]
    [InlineData(TipoDireitoDocumentado.PropriedadePlena)]
    [InlineData(TipoDireitoDocumentado.NuaPropriedade)]
    [InlineData(TipoDireitoDocumentado.Usufruto)]
    [InlineData(TipoDireitoDocumentado.Onus)]
    [InlineData(TipoDireitoDocumentado.Garantia)]
    public void Direito_PreservaCadaCategoriaSemInferirSemantica(TipoDireitoDocumentado tipo)
    {
        var direito = CriarDireito(tipo: tipo);

        Assert.Equal(tipo, direito.Tipo);
    }

    [Theory]
    [InlineData(ConhecimentoEfeitoDireito.NaoAvaliado)]
    [InlineData(ConhecimentoEfeitoDireito.Conhecido)]
    [InlineData(ConhecimentoEfeitoDireito.Desconhecido)]
    [InlineData(ConhecimentoEfeitoDireito.NaoSuportado)]
    public void Direito_PreservaConhecimentoSemAplicarEfeito(ConhecimentoEfeitoDireito conhecimento)
    {
        var direito = CriarDireito(conhecimentoEfeito: conhecimento);

        Assert.Equal(conhecimento, direito.ConhecimentoEfeito);
        Assert.Equal(ExtensaoDireitoDocumentado.Total, direito.Extensao);
        Assert.Equal([PessoaId], direito.Titulares);
    }

    [Fact]
    public void Direito_RejeitaValoresDeEnumForaDoContrato()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CriarDireito(tipo: (TipoDireitoDocumentado)999));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CriarDireito(conhecimentoEfeito: (ConhecimentoEfeitoDireito)999));
    }

    [Fact]
    public void Extensao_DistingueTotalFracaoENaoIdentificada()
    {
        var metade = ExtensaoDireitoDocumentado.CriarFracaoDoTotal(Fracao.Criar(1, 2));

        Assert.Equal(TipoExtensaoDireitoDocumentado.NaoIdentificada, ExtensaoDireitoDocumentado.NaoIdentificada.Tipo);
        Assert.Null(ExtensaoDireitoDocumentado.NaoIdentificada.FracaoDoTotal);
        Assert.Equal(TipoExtensaoDireitoDocumentado.Total, ExtensaoDireitoDocumentado.Total.Tipo);
        Assert.Equal(Fracao.Um, ExtensaoDireitoDocumentado.Total.FracaoDoTotal);
        Assert.Equal(TipoExtensaoDireitoDocumentado.FracaoDoTotal, metade.Tipo);
        Assert.Equal(Fracao.Criar(1, 2), metade.FracaoDoTotal);
        Assert.Equal("1/2", metade.ToString());
    }

    [Fact]
    public void Extensao_CanonicalizaUmComoTotalERejeitaLimitesInvalidos()
    {
        Assert.Equal(
            ExtensaoDireitoDocumentado.Total,
            ExtensaoDireitoDocumentado.CriarFracaoDoTotal(Fracao.Um));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ExtensaoDireitoDocumentado.CriarFracaoDoTotal(Fracao.Zero));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ExtensaoDireitoDocumentado.CriarFracaoDoTotal(Fracao.Criar(3, 2)));
    }

    [Fact]
    public void Direito_DerivaContextoDoAtoEMantemDatasIndependentes()
    {
        var inicio = new DateOnly(2026, 8, 20);
        var fimAnterior = new DateOnly(2025, 2, 10);
        var direito = CriarDireito(inicioDocumentado: inicio, fimDocumentado: fimAnterior);

        Assert.Equal(DireitoId, direito.Id);
        Assert.Equal(AtoId, direito.AtoOrigemId);
        Assert.Equal(MatriculaId, direito.MatriculaId);
        Assert.Equal(FonteId, direito.FonteId);
        Assert.Equal(VersaoFonte.Criar(1), direito.VersaoFonte);
        Assert.Equal(inicio, direito.InicioDocumentado);
        Assert.Equal(fimAnterior, direito.FimDocumentado);

        var semDatas = CriarDireito(inicioDocumentado: null, fimDocumentado: null);
        Assert.Null(semDatas.InicioDocumentado);
        Assert.Null(semDatas.FimDocumentado);
    }

    [Fact]
    public void Direito_AceitaTitularesVaziosERejeitaInvalidosOuDuplicados()
    {
        Assert.Empty(CriarDireito(titulares: []).Titulares);
        Assert.Empty(CriarDireito(titulares: []).ChavesTitularidade);
        Assert.Throws<ArgumentException>(() => CriarDireito(titulares: [default]));
        Assert.Throws<ArgumentException>(() => CriarDireito(titulares: [PessoaId, PessoaId]));
    }

    [Fact]
    public void Direito_CopiaColecoesDefensivamente()
    {
        var contexto = CriarContexto();
        var outroTitular = PessoaId.Criar(Guid.Parse("50000000-0000-4000-8000-000000000002"));
        var titulares = new[] { PessoaId };
        var evidencias = new[] { contexto.Evidencia };
        var direito = CriarDireito(contexto, titulares: titulares, evidencias: evidencias);

        titulares[0] = outroTitular;
        evidencias[0] = CriarEvidencia(contexto.Fonte, Pagina.Criar(1), "Outro texto");

        Assert.Equal(PessoaId, direito.Titulares[0]);
        Assert.Equal(contexto.Evidencia, direito.Evidencias[0]);
        Assert.Equal(PessoaId, direito.ChavesTitularidade[0].PessoaId);
    }

    [Fact]
    public void Direito_ExigeEvidenciaUnica()
    {
        var contexto = CriarContexto();

        Assert.Throws<ArgumentException>(() => CriarDireito(contexto, evidencias: []));
        Assert.Throws<ArgumentException>(
            () => CriarDireito(contexto, evidencias: [contexto.Evidencia, contexto.Evidencia]));
    }

    [Fact]
    public void Direito_RejeitaEvidenciaDeOutraFonteVersaoOuPagina()
    {
        var contexto = CriarContexto();
        var outraFonte = CriarFonte(
            FonteDocumentalId.Criar(Guid.Parse("40000000-0000-4000-8000-000000000002")),
            VersaoFonte.Criar(1));
        var outraVersao = CriarFonte(FonteId, VersaoFonte.Criar(2));
        var evidenciaOutraFonte = CriarEvidencia(outraFonte, Pagina.Criar(1), "Fonte diversa");
        var evidenciaOutraVersao = CriarEvidencia(outraVersao, Pagina.Criar(1), "Versão diversa");
        var evidenciaOutraPagina = CriarEvidencia(contexto.Fonte, Pagina.Criar(2), "Página diversa");

        Assert.Throws<ArgumentException>(() => CriarDireito(contexto, evidencias: [evidenciaOutraFonte]));
        Assert.Throws<ArgumentException>(() => CriarDireito(contexto, evidencias: [evidenciaOutraVersao]));
        Assert.Throws<ArgumentException>(() => CriarDireito(contexto, evidencias: [evidenciaOutraPagina]));
    }

    [Fact]
    public void ChaveTitularidade_SeparaCategoriasDoMesmoTitular()
    {
        var propriedade = CriarDireito(tipo: TipoDireitoDocumentado.PropriedadePlena);
        var nuaPropriedade = CriarDireito(tipo: TipoDireitoDocumentado.NuaPropriedade);
        var usufruto = CriarDireito(tipo: TipoDireitoDocumentado.Usufruto);
        var chaves = new[]
        {
            propriedade.ChavesTitularidade.Single(),
            nuaPropriedade.ChavesTitularidade.Single(),
            usufruto.ChavesTitularidade.Single(),
        };

        Assert.Equal(3, chaves.Distinct().Count());
        Assert.All(chaves, chave => Assert.Equal(MatriculaId, chave.MatriculaId));
        Assert.All(chaves, chave => Assert.Equal(PessoaId, chave.PessoaId));
    }

    private static DireitoDocumentado CriarDireito(
        TipoDireitoDocumentado tipo = TipoDireitoDocumentado.PropriedadePlena,
        ConhecimentoEfeitoDireito conhecimentoEfeito = ConhecimentoEfeitoDireito.NaoAvaliado,
        DateOnly? inicioDocumentado = null,
        DateOnly? fimDocumentado = null,
        IEnumerable<PessoaId>? titulares = null)
    {
        return CriarDireito(
            CriarContexto(),
            tipo,
            conhecimentoEfeito,
            inicioDocumentado,
            fimDocumentado,
            titulares);
    }

    private static DireitoDocumentado CriarDireito(
        Contexto contexto,
        TipoDireitoDocumentado tipo = TipoDireitoDocumentado.PropriedadePlena,
        ConhecimentoEfeitoDireito conhecimentoEfeito = ConhecimentoEfeitoDireito.NaoAvaliado,
        DateOnly? inicioDocumentado = null,
        DateOnly? fimDocumentado = null,
        IEnumerable<PessoaId>? titulares = null,
        IEnumerable<TrechoEvidencia>? evidencias = null)
    {
        return DireitoDocumentado.Criar(
            DireitoId,
            contexto.Ato,
            tipo,
            ExtensaoDireitoDocumentado.Total,
            conhecimentoEfeito,
            inicioDocumentado,
            fimDocumentado,
            titulares ?? [PessoaId],
            evidencias ?? [contexto.Evidencia]);
    }

    private static Contexto CriarContexto()
    {
        const string textoPagina = "Pessoa Sintética possui metade documentada.";
        var fonte = CriarFonte(FonteId, VersaoFonte.Criar(1));
        var pagina = Pagina.Criar(1);
        var evidencia = CriarEvidencia(fonte, pagina, textoPagina);
        var ato = AtoRegistral.Criar(
            AtoId,
            MatriculaId,
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
            "matricula-sintetica.pdf",
            "application/pdf",
            512);
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
