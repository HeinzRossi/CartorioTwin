using System.Text.Json;
using CartorioTwin.Domain.Documentos;

namespace CartorioTwin.Domain.Tests;

public sealed class CompatibilidadeRevisaoFixturesTests
{
    private static readonly string[] FixturesEsperadas =
    [
        "GD-001-native-text.json",
        "GD-002-ocr-low-quality.json",
        "GD-003-multipage-relation.json",
        "GD-004-unsupported.json",
        "GD-005-ct001-divergence.json",
    ];

    [Theory]
    [InlineData("extracted", EstadoRevisaoFato.Extraido, SituacaoEvidenciaFato.Localizada)]
    [InlineData("under_review", EstadoRevisaoFato.EmRevisao, SituacaoEvidenciaFato.Localizada)]
    [InlineData("confirmed", EstadoRevisaoFato.Confirmado, SituacaoEvidenciaFato.Localizada)]
    [InlineData("contested", EstadoRevisaoFato.Contestado, SituacaoEvidenciaFato.Localizada)]
    [InlineData("unsupported", EstadoRevisaoFato.NaoSuportado, SituacaoEvidenciaFato.Localizada)]
    [InlineData("not_found", EstadoRevisaoFato.Contestado, SituacaoEvidenciaFato.NaoLocalizada)]
    [InlineData("declared_without_evidence", EstadoRevisaoFato.Confirmado, SituacaoEvidenciaFato.DeclaradaSemEvidencia)]
    public void EstadosExternos_MapeiamLifecycleEProvenienciaSeparados(
        string estadoExterno,
        EstadoRevisaoFato estado,
        SituacaoEvidenciaFato situacao)
    {
        Assert.Equal((estado, situacao), MapearEstado(estadoExterno));
    }

    [Theory]
    [InlineData("confirmed", DecisaoRevisaoFato.Confirmar)]
    [InlineData("corrected", DecisaoRevisaoFato.Corrigir)]
    [InlineData("contested", DecisaoRevisaoFato.Contestar)]
    [InlineData("not_found", DecisaoRevisaoFato.MarcarNaoLocalizado)]
    [InlineData("declared_without_evidence", DecisaoRevisaoFato.DeclararSemEvidencia)]
    public void DecisoesExternas_MapeiamContratoDeDominio(string decisaoExterna, DecisaoRevisaoFato decisao)
    {
        Assert.Equal(decisao, MapearDecisao(decisaoExterna));
    }

    [Fact]
    public void CincoFixtures_UsamSomenteEstadosEDecisoesMapeados()
    {
        var diretorio = Path.Combine(AppContext.BaseDirectory, "ContractFixtures");
        var arquivos = Directory.GetFiles(diretorio, "*.json")
            .OrderBy(Path.GetFileName, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(FixturesEsperadas, arquivos.Select(caminho => Path.GetFileName(caminho)!).ToArray());

        var casos = new HashSet<string>(StringComparer.Ordinal);
        var campos = 0;
        var revisoes = 0;
        foreach (var arquivo in arquivos)
        {
            using var documento = JsonDocument.Parse(File.ReadAllText(arquivo));
            var raiz = documento.RootElement;
            Assert.True(casos.Add(raiz.GetProperty("caseId").GetString()!));

            foreach (var ato in raiz.GetProperty("acts").EnumerateArray())
            {
                foreach (var campo in ato.GetProperty("fields").EnumerateArray())
                {
                    var estado = campo.GetProperty("state").GetString()!;
                    _ = MapearEstado(estado);
                    campos++;
                }
            }

            foreach (var revisao in raiz.GetProperty("reviews").EnumerateArray())
            {
                var decisao = revisao.GetProperty("decision").GetString()!;
                _ = MapearDecisao(decisao);
                revisoes++;
            }
        }

        Assert.Equal(5, casos.Count);
        Assert.True(campos > 0);
        Assert.True(revisoes > 0);
    }

    private static (EstadoRevisaoFato Estado, SituacaoEvidenciaFato Situacao) MapearEstado(string estado)
    {
        return estado switch
        {
            "extracted" => (EstadoRevisaoFato.Extraido, SituacaoEvidenciaFato.Localizada),
            "under_review" => (EstadoRevisaoFato.EmRevisao, SituacaoEvidenciaFato.Localizada),
            "confirmed" => (EstadoRevisaoFato.Confirmado, SituacaoEvidenciaFato.Localizada),
            "contested" => (EstadoRevisaoFato.Contestado, SituacaoEvidenciaFato.Localizada),
            "unsupported" => (EstadoRevisaoFato.NaoSuportado, SituacaoEvidenciaFato.Localizada),
            "not_found" => (EstadoRevisaoFato.Contestado, SituacaoEvidenciaFato.NaoLocalizada),
            "declared_without_evidence" => (EstadoRevisaoFato.Confirmado, SituacaoEvidenciaFato.DeclaradaSemEvidencia),
            _ => throw new InvalidOperationException($"Estado externo não mapeado: {estado}."),
        };
    }

    private static DecisaoRevisaoFato MapearDecisao(string decisao)
    {
        return decisao switch
        {
            "confirmed" => DecisaoRevisaoFato.Confirmar,
            "corrected" => DecisaoRevisaoFato.Corrigir,
            "contested" => DecisaoRevisaoFato.Contestar,
            "not_found" => DecisaoRevisaoFato.MarcarNaoLocalizado,
            "declared_without_evidence" => DecisaoRevisaoFato.DeclararSemEvidencia,
            _ => throw new InvalidOperationException($"Decisão externa não mapeada: {decisao}."),
        };
    }
}
