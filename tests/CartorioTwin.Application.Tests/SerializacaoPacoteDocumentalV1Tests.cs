using System.Text.Json;
using System.Text.Json.Nodes;
using CartorioTwin.Application.Documentos.V1;
using CartorioTwin.Contracts.Documentos.V1;
using CartorioTwin.Domain.Documentos;

namespace CartorioTwin.Application.Tests;

public sealed class SerializacaoPacoteDocumentalV1Tests
{
    private static string CaminhoFixture =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "PKG-001-full-roundtrip.json");

    [Fact]
    public void FixtureCompleta_FazRoundTripSemPerderIntegridade()
    {
        var json = File.ReadAllText(CaminhoFixture);

        var primeiraLeitura = SerializadorPacoteDocumentalV1.Desserializar(json);

        Assert.True(primeiraLeitura.Sucesso, MensagemErros(primeiraLeitura));
        var pacote = Assert.IsType<PacoteDocumentalReidratadoV1>(primeiraLeitura.Pacote);
        Assert.Single(pacote.Fontes);
        Assert.Equal(2, pacote.Paginas.Count);
        Assert.Equal(2, pacote.Atos.Count);
        Assert.Single(pacote.Direitos);
        Assert.Equal(7, pacote.Fatos.Count);
        Assert.Single(pacote.Atos[1].Relacoes);
        Assert.Equal("propriedade plena", pacote.Direitos[0].Evidencias.Single().Trecho);

        var jsonReescrito = SerializadorPacoteDocumentalV1.Serializar(pacote);
        var segundaLeitura = SerializadorPacoteDocumentalV1.Desserializar(jsonReescrito);

        Assert.True(segundaLeitura.Sucesso, MensagemErros(segundaLeitura));
        var segundoPacote = Assert.IsType<PacoteDocumentalReidratadoV1>(segundaLeitura.Pacote);
        Assert.Equal(pacote.Fontes, segundoPacote.Fontes);
        Assert.Equal(pacote.Paginas, segundoPacote.Paginas);
        Assert.Equal(pacote.Atos.Select(ato => ato.Id), segundoPacote.Atos.Select(ato => ato.Id));
        Assert.Equal(pacote.Direitos.Select(direito => direito.Id), segundoPacote.Direitos.Select(direito => direito.Id));
        Assert.Equal(pacote.Fatos.Select(fato => fato.Id), segundoPacote.Fatos.Select(fato => fato.Id));
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(jsonReescrito), JsonNode.Parse(
            SerializadorPacoteDocumentalV1.Serializar(segundoPacote))));
    }

    [Fact]
    public void FixtureCompleta_ExercitaUniaoFechadaEProveniencias()
    {
        var resultado = SerializadorPacoteDocumentalV1.Desserializar(File.ReadAllText(CaminhoFixture));
        var pacote = Assert.IsType<PacoteDocumentalReidratadoV1>(resultado.Pacote);

        var tipos = pacote.Fatos
            .SelectMany(fato => new[]
            {
                fato.ValorSugerido.PossuiValor ? fato.ValorSugerido.Valor.Tipo : (TipoValorFatoContratoV1?)null,
                fato.ValorFinal.PossuiValor ? fato.ValorFinal.Valor.Tipo : (TipoValorFatoContratoV1?)null,
            })
            .OfType<TipoValorFatoContratoV1>()
            .ToHashSet();

        Assert.Equal(Enum.GetValues<TipoValorFatoContratoV1>().ToHashSet(), tipos);
        Assert.Contains(pacote.Fatos, fato => fato.SituacaoEvidencia == SituacaoEvidenciaFato.Localizada);
        Assert.Contains(pacote.Fatos, fato => fato.SituacaoEvidencia == SituacaoEvidenciaFato.NaoLocalizada);
        Assert.Contains(pacote.Fatos, fato => fato.SituacaoEvidencia == SituacaoEvidenciaFato.DeclaradaSemEvidencia);
        Assert.Contains(pacote.Fatos, fato => fato.Versao.Numero == 2 && fato.FatoAnteriorId is not null);
    }

    [Fact]
    public void Pacote_AceitaMultiplasFontesEWhitespaceIrrelevante()
    {
        var raiz = LerRaiz();
        var fontes = raiz["fontes"]!.AsArray();
        var fonte2 = fontes[0]!.DeepClone().AsObject();
        fonte2["id"] = "10000000-0000-4000-8000-000000000002";
        fonte2["hashSha256"] = "sha256:2222222222222222222222222222222222222222222222222222222222222222";
        fonte2["nomeSanitizado"] = "anexo-sintetico.pdf";
        fontes.Add(fonte2);

        var pagina2 = raiz["paginas"]![0]!.DeepClone().AsObject();
        pagina2["fonteId"] = "10000000-0000-4000-8000-000000000002";
        raiz["paginas"]!.AsArray().Add(pagina2);

        var jsonCompacto = raiz.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        var resultado = SerializadorPacoteDocumentalV1.Desserializar(jsonCompacto);

        Assert.True(resultado.Sucesso, MensagemErros(resultado));
        Assert.Equal(2, resultado.Pacote!.Fontes.Count);
        Assert.Equal(3, resultado.Pacote.Paginas.Count);
    }

    [Theory]
    [InlineData("versao", CodigoErroPacoteV1.VersaoNaoSuportada)]
    [InlineData("propriedade_desconhecida", CodigoErroPacoteV1.JsonInvalido)]
    [InlineData("enum_desconhecido", CodigoErroPacoteV1.JsonInvalido)]
    [InlineData("guid_maiusculo", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("hash_texto", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("fonte_ausente", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("fonte_duplicada", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("pagina_duplicada", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("fato_duplicado", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("revisao_duplicada", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("ato_relacionado_ausente", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("cadeia_salto", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("cadeia_ciclo", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("estado_incompatível", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("valor_incompatível", CodigoErroPacoteV1.ReferenciaInvalida)]
    [InlineData("offset_incorreto", CodigoErroPacoteV1.InvarianteDominio)]
    [InlineData("revisao_outro_fato", CodigoErroPacoteV1.InvarianteDominio)]
    public void Pacote_RejeitaMutacoesInvalidas(string mutacao, CodigoErroPacoteV1 codigoEsperado)
    {
        var raiz = LerRaiz();
        AplicarMutacao(raiz, mutacao);

        var resultado = SerializadorPacoteDocumentalV1.Desserializar(raiz.ToJsonString());

        Assert.False(resultado.Sucesso);
        Assert.Null(resultado.Pacote);
        Assert.Equal(codigoEsperado, Assert.Single(resultado.Erros).Codigo);
    }

    private static void AplicarMutacao(JsonObject raiz, string mutacao)
    {
        switch (mutacao)
        {
            case "versao":
                raiz["schemaVersion"] = "2.0";
                break;
            case "propriedade_desconhecida":
                raiz["campoNovo"] = true;
                break;
            case "enum_desconhecido":
                raiz["atos"]![0]!["identificador"]!["tipo"] = "outro";
                break;
            case "guid_maiusculo":
                raiz["fontes"]![0]!["id"] = "10000000-0000-4000-8000-00000000000A";
                break;
            case "hash_texto":
                raiz["paginas"]![0]!["textoHashSha256"] =
                    "sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                break;
            case "fonte_ausente":
                raiz["atos"]![0]!["fonteId"] = "10000000-0000-4000-8000-000000000099";
                break;
            case "fonte_duplicada":
                raiz["fontes"]!.AsArray().Add(raiz["fontes"]![0]!.DeepClone());
                break;
            case "pagina_duplicada":
                raiz["paginas"]!.AsArray().Add(raiz["paginas"]![0]!.DeepClone());
                break;
            case "fato_duplicado":
                raiz["fatos"]!.AsArray().Add(raiz["fatos"]![0]!.DeepClone());
                break;
            case "revisao_duplicada":
                raiz["revisoes"]!.AsArray().Add(raiz["revisoes"]![0]!.DeepClone());
                break;
            case "ato_relacionado_ausente":
                raiz["atos"]![1]!["relacoes"]![0]!["atoRelacionadoId"] =
                    "20000000-0000-4000-8000-000000000099";
                break;
            case "cadeia_salto":
                raiz["fatos"]![1]!["versao"] = 3;
                break;
            case "cadeia_ciclo":
                raiz["fatos"]![0]!["fatoAnteriorId"] = "60000000-0000-4000-8000-000000000002";
                break;
            case "estado_incompatível":
                raiz["fatos"]![0]!["estado"] = "contestado";
                break;
            case "valor_incompatível":
                raiz["fatos"]![0]!["valorSugerido"]!["tipo"] = "inteiro";
                break;
            case "offset_incorreto":
                raiz["fatos"]![0]!["evidencias"]![0]!["fimUtf16"] = 500;
                break;
            case "revisao_outro_fato":
                raiz["revisoes"]![0]!["fatoId"] = "60000000-0000-4000-8000-000000000003";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mutacao));
        }
    }

    private static JsonObject LerRaiz() =>
        JsonNode.Parse(File.ReadAllText(CaminhoFixture))!.AsObject();

    private static string MensagemErros(ResultadoLeituraPacoteV1 resultado) =>
        string.Join(" | ", resultado.Erros.Select(erro => $"{erro.Codigo}:{erro.Caminho}:{erro.Mensagem}"));
}
