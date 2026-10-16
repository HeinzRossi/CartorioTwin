using System.Text.Json;
using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Tests;

public sealed class CompatibilidadeFixturesTests
{
    private static readonly string[] FixturesEsperadas =
    [
        "GD-001-native-text.json",
        "GD-002-ocr-low-quality.json",
        "GD-003-multipage-relation.json",
        "GD-004-unsupported.json",
        "GD-005-ct001-divergence.json",
    ];

    [Fact]
    public void FixturesMapeiamFontesETrechosParaODominioCandidato()
    {
        var diretorio = Path.Combine(AppContext.BaseDirectory, "ContractFixtures");
        var arquivos = Directory.GetFiles(diretorio, "*.json")
            .OrderBy(Path.GetFileName, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(FixturesEsperadas, arquivos.Select(caminho => Path.GetFileName(caminho)!).ToArray());

        var casosLidos = new HashSet<string>(StringComparer.Ordinal);
        var evidenciasLidas = 0;

        foreach (var arquivo in arquivos)
        {
            using var documento = JsonDocument.Parse(File.ReadAllText(arquivo));
            var raiz = documento.RootElement;
            Assert.True(raiz.GetProperty("synthetic").GetBoolean());
            Assert.True(casosLidos.Add(raiz.GetProperty("caseId").GetString()!));

            var fonteJson = raiz.GetProperty("source");
            var fonte = FonteDocumental.Criar(
                FonteDocumentalId.Criar(fonteJson.GetProperty("id").GetGuid()),
                VersaoFonte.Criar(fonteJson.GetProperty("version").GetInt64()),
                HashSha256.CriarCanonico(fonteJson.GetProperty("sha256").GetString()!),
                fonteJson.GetProperty("nameSanitized").GetString()!,
                fonteJson.GetProperty("mimeType").GetString()!,
                fonteJson.GetProperty("byteLength").GetInt64());

            Assert.Equal(fonteJson.GetProperty("id").GetGuid(), fonte.Id.Valor);
            Assert.Equal(fonteJson.GetProperty("version").GetInt64(), fonte.Versao.Numero);
            Assert.Equal(fonteJson.GetProperty("sha256").GetString(), fonte.Hash.ValorCanonico);
            Assert.Equal(fonteJson.GetProperty("nameSanitized").GetString(), fonte.NomeOriginalSanitizado);
            Assert.Equal("application/pdf", fonte.MimeTypeValidado);
            Assert.Equal(fonteJson.GetProperty("byteLength").GetInt64(), fonte.TamanhoBytes);

            var paginas = raiz.GetProperty("pages").EnumerateArray()
                .ToDictionary(pagina => pagina.GetProperty("number").GetInt32());

            foreach (var ato in raiz.GetProperty("acts").EnumerateArray())
            {
                foreach (var campo in ato.GetProperty("fields").EnumerateArray())
                {
                    foreach (var evidenciaJson in campo.GetProperty("evidences").EnumerateArray())
                    {
                        var numeroPagina = evidenciaJson.GetProperty("page").GetInt32();
                        var paginaJson = paginas[numeroPagina];
                        var evidencia = TrechoEvidencia.Criar(
                            fonte,
                            Pagina.Criar(numeroPagina),
                            paginaJson.GetProperty("text").GetString()!,
                            evidenciaJson.GetProperty("startUtf16").GetInt32(),
                            evidenciaJson.GetProperty("endUtf16").GetInt32());

                        Assert.Equal(evidenciaJson.GetProperty("sourceId").GetGuid(), evidencia.FonteId.Valor);
                        Assert.Equal(evidenciaJson.GetProperty("sourceVersion").GetInt64(), evidencia.VersaoFonte.Numero);
                        Assert.Equal(evidenciaJson.GetProperty("sourceSha256").GetString(), evidencia.HashFonte.ValorCanonico);
                        Assert.Equal(numeroPagina, evidencia.Pagina.Numero);
                        Assert.Equal(evidenciaJson.GetProperty("textSha256").GetString(), evidencia.HashTextoPagina.ValorCanonico);
                        Assert.Equal(evidenciaJson.GetProperty("startUtf16").GetInt32(), evidencia.InicioUtf16);
                        Assert.Equal(evidenciaJson.GetProperty("endUtf16").GetInt32(), evidencia.FimUtf16);
                        Assert.Equal(evidenciaJson.GetProperty("excerpt").GetString(), evidencia.Trecho);
                        evidenciasLidas++;
                    }
                }
            }
        }

        Assert.Equal(5, casosLidos.Count);
        Assert.True(evidenciasLidas > 0);
    }

    [Fact]
    public void Gd003MapeiaOrdemPaginasERelacaoEntreAtos()
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "ContractFixtures", "GD-003-multipage-relation.json");
        using var documento = JsonDocument.Parse(File.ReadAllText(caminho));
        var raiz = documento.RootElement;
        var fonteJson = raiz.GetProperty("source");
        var fonteId = FonteDocumentalId.Criar(fonteJson.GetProperty("id").GetGuid());
        var versaoFonte = VersaoFonte.Criar(fonteJson.GetProperty("version").GetInt64());
        var matriculaId = MatriculaId.Criar(Guid.Parse("33333333-3333-4333-8333-333333339999"));

        var atos = raiz.GetProperty("acts").EnumerateArray()
            .Select(atoJson => CriarAtoDaFixture(atoJson, matriculaId, fonteId, versaoFonte))
            .ToArray();

        Assert.Equal(2, atos.Length);
        Assert.Equal("R-3", atos[0].Identificador.ValorCanonico);
        Assert.Equal(1, atos[0].OrdemNaFonte.Numero);
        Assert.Equal([Pagina.Criar(1)], atos[0].Paginas);
        Assert.Equal("AV-4", atos[1].Identificador.ValorCanonico);
        Assert.Equal(2, atos[1].OrdemNaFonte.Numero);
        Assert.Equal([Pagina.Criar(1), Pagina.Criar(2)], atos[1].Paginas);
        var relacao = Assert.Single(atos[1].Relacoes);
        Assert.Equal(atos[0].Id, relacao.AtoRelacionadoId);
        Assert.Equal(TipoRelacaoAto.Referencia, relacao.Tipo);
        Assert.Equal(DatasDocumentaisAto.Nenhuma, atos[0].Datas);
        Assert.Empty(atos[0].Partes);
    }

    private static AtoRegistral CriarAtoDaFixture(
        JsonElement atoJson,
        MatriculaId matriculaId,
        FonteDocumentalId fonteId,
        VersaoFonte versaoFonte)
    {
        var identificadorOriginal = atoJson.GetProperty("identifier").GetString()!;
        var partesIdentificador = identificadorOriginal.Split('-', 2);
        var numero = long.Parse(partesIdentificador[1], System.Globalization.CultureInfo.InvariantCulture);
        var identificador = partesIdentificador[0] switch
        {
            "R" => IdentificadorAtoRegistral.CriarRegistro(numero, identificadorOriginal),
            "AV" => IdentificadorAtoRegistral.CriarAverbacao(numero, identificadorOriginal),
            _ => IdentificadorAtoRegistral.CriarNaoIdentificado(identificadorOriginal),
        };
        var relacoes = atoJson.GetProperty("relations").EnumerateArray()
            .Select(relacaoJson => RelacaoAtoRegistral.Criar(
                AtoRegistralId.Criar(relacaoJson.GetProperty("targetActId").GetGuid()),
                relacaoJson.GetProperty("type").GetString() == "reference"
                    ? TipoRelacaoAto.Referencia
                    : TipoRelacaoAto.Desconhecida));

        return AtoRegistral.Criar(
            AtoRegistralId.Criar(atoJson.GetProperty("id").GetGuid()),
            matriculaId,
            fonteId,
            versaoFonte,
            identificador,
            OrdemAtoRegistral.Criar(atoJson.GetProperty("order").GetInt64()),
            DatasDocumentaisAto.Nenhuma,
            atoJson.GetProperty("pages").EnumerateArray().Select(pagina => Pagina.Criar(pagina.GetInt32())),
            [],
            relacoes);
    }
}
