using System.Text.Json;
using CartorioTwin.Domain.Documentos;

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
}
