using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Tests;

public sealed class AtoRegistralTests
{
    private static readonly AtoRegistralId AtoId = AtoRegistralId.Criar(
        Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa1"));
    private static readonly MatriculaId MatriculaId = MatriculaId.Criar(
        Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbb1"));
    private static readonly FonteDocumentalId FonteId = FonteDocumentalId.Criar(
        Guid.Parse("cccccccc-cccc-4ccc-8ccc-ccccccccccc1"));

    [Fact]
    public void AtoRegistralId_RejeitaVazioEMantemIgualdade()
    {
        var valor = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa2");

        Assert.Throws<ArgumentOutOfRangeException>(() => AtoRegistralId.Criar(Guid.Empty));
        Assert.Equal(AtoRegistralId.Criar(valor), AtoRegistralId.Criar(valor));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrdemAtoRegistral_RejeitaNumeroNaoPositivo(long numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => OrdemAtoRegistral.Criar(numero));
    }

    [Fact]
    public void Identificador_DistingueRegistroAverbacaoEDesconhecido()
    {
        var registro = IdentificadorAtoRegistral.CriarRegistro(10, " R-010 ");
        var averbacao = IdentificadorAtoRegistral.CriarAverbacao(4);
        var desconhecido = IdentificadorAtoRegistral.CriarNaoIdentificado("Ato sem marcador");

        Assert.Equal(TipoIdentificadorAto.Registro, registro.Tipo);
        Assert.Equal(10, registro.Numero);
        Assert.Equal("R-10", registro.ValorCanonico);
        Assert.Equal("R-010", registro.TextoOriginal);
        Assert.Equal(TipoIdentificadorAto.Averbacao, averbacao.Tipo);
        Assert.Equal("AV-4", averbacao.ValorCanonico);
        Assert.Equal(TipoIdentificadorAto.NaoIdentificado, desconhecido.Tipo);
        Assert.Null(desconhecido.Numero);
        Assert.Null(desconhecido.ValorCanonico);
        Assert.Equal("Ato sem marcador", desconhecido.ToString());
    }

    [Fact]
    public void Identificador_RejeitaNumeroOuTextoInvalido()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IdentificadorAtoRegistral.CriarRegistro(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => IdentificadorAtoRegistral.CriarAverbacao(-1));
        Assert.Throws<ArgumentException>(() => IdentificadorAtoRegistral.CriarNaoIdentificado(" "));
        Assert.Throws<ArgumentException>(() => IdentificadorAtoRegistral.CriarRegistro(1, "R-\n1"));
    }

    [Fact]
    public void DatasDocumentais_PermanecemSeparadasESemOrdemInferida()
    {
        var dataTitulo = new DateOnly(2025, 3, 20);
        var dataAto = new DateOnly(2025, 3, 10);
        var dataApresentacao = new DateOnly(2025, 3, 15);
        var datas = new DatasDocumentaisAto(dataTitulo, dataAto, dataApresentacao);

        Assert.Equal(dataTitulo, datas.DataDoTitulo);
        Assert.Equal(dataAto, datas.DataDoAto);
        Assert.Equal(dataApresentacao, datas.DataDaApresentacao);
        Assert.Null(DatasDocumentaisAto.Nenhuma.DataDoTitulo);
        Assert.Null(DatasDocumentaisAto.Nenhuma.DataDoAto);
        Assert.Null(DatasDocumentaisAto.Nenhuma.DataDaApresentacao);
    }

    [Fact]
    public void Partes_NaoUsamRotuloComoIdentidade()
    {
        const string rotulo = "Pessoa Alfa";
        var primeira = ParteAtoRegistral.CriarIdentificada(
            PessoaId.Criar(Guid.Parse("dddddddd-dddd-4ddd-8ddd-ddddddddddd1")),
            rotulo);
        var segunda = ParteAtoRegistral.CriarIdentificada(
            PessoaId.Criar(Guid.Parse("dddddddd-dddd-4ddd-8ddd-ddddddddddd2")),
            rotulo);
        var naoIdentificada = ParteAtoRegistral.CriarNaoIdentificada($" {rotulo} ");

        Assert.NotEqual(primeira, segunda);
        Assert.Null(naoIdentificada.PessoaId);
        Assert.Equal(rotulo, naoIdentificada.RotuloDocumental);
        Assert.Throws<ArgumentException>(() => ParteAtoRegistral.CriarNaoIdentificada(" "));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => ParteAtoRegistral.CriarIdentificada(default, rotulo));
    }

    [Fact]
    public void Relacao_RejeitaAlvoVazioETipoForaDoContrato()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RelacaoAtoRegistral.Criar(default, TipoRelacaoAto.Referencia));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RelacaoAtoRegistral.Criar(AtoId, (TipoRelacaoAto)999));

        var desconhecida = RelacaoAtoRegistral.Criar(
            AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa3")),
            TipoRelacaoAto.Desconhecida);
        Assert.Equal(TipoRelacaoAto.Desconhecida, desconhecida.Tipo);
    }

    [Theory]
    [InlineData(TipoRelacaoAto.Referencia)]
    [InlineData(TipoRelacaoAto.Retificacao)]
    [InlineData(TipoRelacaoAto.Cancelamento)]
    [InlineData(TipoRelacaoAto.Correspondencia)]
    [InlineData(TipoRelacaoAto.Desconhecida)]
    public void Relacao_PreservaCadaTipoSemInferirEfeito(TipoRelacaoAto tipo)
    {
        var alvo = AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa8"));

        var relacao = RelacaoAtoRegistral.Criar(alvo, tipo);

        Assert.Equal(alvo, relacao.AtoRelacionadoId);
        Assert.Equal(tipo, relacao.Tipo);
    }

    [Fact]
    public void AtoRegistral_PreservaContextoESeparaMarcadorDaOrdem()
    {
        var ato = CriarAto(
            identificador: IdentificadorAtoRegistral.CriarRegistro(10),
            ordem: OrdemAtoRegistral.Criar(2));

        Assert.Equal(AtoId, ato.Id);
        Assert.Equal(MatriculaId, ato.MatriculaId);
        Assert.Equal(FonteId, ato.FonteId);
        Assert.Equal(1, ato.VersaoFonte.Numero);
        Assert.Equal(10, ato.Identificador.Numero);
        Assert.Equal(2, ato.OrdemNaFonte.Numero);
        Assert.Empty(ato.Partes);
        Assert.Empty(ato.Relacoes);
    }

    [Fact]
    public void AtoRegistral_PreservaMarcadorNaoIdentificado()
    {
        var ato = CriarAto(
            identificador: IdentificadorAtoRegistral.CriarNaoIdentificado("Marcador ilegível"));

        Assert.Equal(TipoIdentificadorAto.NaoIdentificado, ato.Identificador.Tipo);
        Assert.Equal("Marcador ilegível", ato.Identificador.TextoOriginal);
        Assert.Null(ato.Identificador.ValorCanonico);
    }

    [Fact]
    public void AtoRegistral_RejeitaPaginasERelacoesInvalidas()
    {
        Assert.Throws<ArgumentException>(() => CriarAto(paginas: []));
        Assert.Throws<ArgumentException>(
            () => CriarAto(paginas: [Pagina.Criar(1), Pagina.Criar(1)]));
        Assert.Throws<ArgumentException>(
            () => CriarAto(relacoes: [RelacaoAtoRegistral.Criar(AtoId, TipoRelacaoAto.Referencia)]));

        var alvo = AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa4"));
        var relacao = RelacaoAtoRegistral.Criar(alvo, TipoRelacaoAto.Referencia);
        Assert.Throws<ArgumentException>(() => CriarAto(relacoes: [relacao, relacao]));
    }

    [Fact]
    public void AtoRegistral_FazCopiaDefensivaDasColecoes()
    {
        var paginas = new[] { Pagina.Criar(1), Pagina.Criar(2) };
        var partes = new[] { ParteAtoRegistral.CriarNaoIdentificada("Pessoa sintética") };
        var relacoes = new[]
        {
            RelacaoAtoRegistral.Criar(
                AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa5")),
                TipoRelacaoAto.Referencia),
        };
        var ato = CriarAto(paginas: paginas, partes: partes, relacoes: relacoes);

        paginas[0] = Pagina.Criar(9);
        partes[0] = ParteAtoRegistral.CriarNaoIdentificada("Outra pessoa");
        relacoes[0] = RelacaoAtoRegistral.Criar(
            AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa6")),
            TipoRelacaoAto.Cancelamento);

        Assert.Equal(Pagina.Criar(1), ato.Paginas[0]);
        Assert.Equal("Pessoa sintética", ato.Partes[0].RotuloDocumental);
        Assert.Equal(TipoRelacaoAto.Referencia, ato.Relacoes[0].Tipo);
    }

    [Fact]
    public void MesmoMarcadorPodeExistirEmContextosDocumentaisDiferentes()
    {
        var identificador = IdentificadorAtoRegistral.CriarRegistro(5);
        var primeiro = CriarAto(identificador: identificador);
        var segundo = AtoRegistral.Criar(
            AtoRegistralId.Criar(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaa7")),
            MatriculaId.Criar(Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbb2")),
            FonteDocumentalId.Criar(Guid.Parse("cccccccc-cccc-4ccc-8ccc-ccccccccccc2")),
            VersaoFonte.Criar(1),
            identificador,
            OrdemAtoRegistral.Criar(1),
            DatasDocumentaisAto.Nenhuma,
            [Pagina.Criar(1)],
            [],
            []);

        Assert.Equal(primeiro.Identificador, segundo.Identificador);
        Assert.NotEqual(primeiro.MatriculaId, segundo.MatriculaId);
        Assert.NotEqual(primeiro.FonteId, segundo.FonteId);
    }

    private static AtoRegistral CriarAto(
        IdentificadorAtoRegistral? identificador = null,
        OrdemAtoRegistral? ordem = null,
        IEnumerable<Pagina>? paginas = null,
        IEnumerable<ParteAtoRegistral>? partes = null,
        IEnumerable<RelacaoAtoRegistral>? relacoes = null)
    {
        return AtoRegistral.Criar(
            AtoId,
            MatriculaId,
            FonteId,
            VersaoFonte.Criar(1),
            identificador ?? IdentificadorAtoRegistral.CriarRegistro(1),
            ordem ?? OrdemAtoRegistral.Criar(1),
            DatasDocumentaisAto.Nenhuma,
            paginas ?? [Pagina.Criar(1)],
            partes ?? [],
            relacoes ?? []);
    }
}
