using System.Text;
using CartorioTwin.Domain.Documentos;

namespace CartorioTwin.Domain.Tests;

public sealed class EvidenciaDocumentalTests
{
    private static readonly Guid FonteGuid = Guid.Parse("3a913766-6221-4cfe-938b-253860cc66cb");

    [Fact]
    public void FonteDocumentalId_RejeitaVazioEMantemIgualdade()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FonteDocumentalId.Criar(Guid.Empty));
        Assert.Equal(FonteDocumentalId.Criar(FonteGuid), FonteDocumentalId.Criar(FonteGuid));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Pagina_RejeitaNumeroNaoPositivo(int numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Pagina.Criar(numero));
    }

    [Fact]
    public void Pagina_AceitaNumeroPositivo()
    {
        Assert.Equal(2, Pagina.Criar(2).Numero);
        Assert.Equal("2", Pagina.Criar(2).ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void VersaoFonte_RejeitaNumeroNaoPositivo(long numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => VersaoFonte.Criar(numero));
    }

    [Fact]
    public void VersaoFonte_AceitaNumeroPositivo()
    {
        Assert.Equal(3, VersaoFonte.Criar(3).Numero);
        Assert.Equal("3", VersaoFonte.Criar(3).ToString());
    }

    [Fact]
    public void HashSha256_NormalizaHexadecimalEConfereVetorConhecido()
    {
        const string esperado = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad";

        Assert.Equal(esperado, HashSha256.Criar(esperado.ToUpperInvariant()).Valor);
        Assert.Equal(esperado, HashSha256.Calcular(Encoding.UTF8.GetBytes("abc")).Valor);
        Assert.Equal(esperado, HashSha256.CalcularUtf8("abc").Valor);
        Assert.Equal($"sha256:{esperado}", HashSha256.CalcularUtf8("abc").ValorCanonico);
        Assert.Equal($"sha256:{esperado}", HashSha256.CalcularUtf8("abc").ToString());
    }

    [Theory]
    [InlineData("0")]
    [InlineData("000000000000000000000000000000000000000000000000000000000000000")]
    [InlineData("gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg")]
    public void HashSha256_RejeitaFormatoInvalido(string valor)
    {
        Assert.Throws<FormatException>(() => HashSha256.Criar(valor));
    }

    [Fact]
    public void HashSha256_CriaDeRepresentacaoCanonicaEstrita()
    {
        const string hexadecimal = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad";
        var hash = HashSha256.CriarCanonico($"sha256:{hexadecimal}");

        Assert.Equal(hexadecimal, hash.Valor);
        Assert.Equal($"sha256:{hexadecimal}", hash.ValorCanonico);
        Assert.Equal(hash.ValorCanonico, hash.ToString());
    }

    [Theory]
    [InlineData("SHA256:ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("sha256:BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD")]
    [InlineData(" sha256:ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("sha256:ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015a")]
    [InlineData("md5:ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("sha256:ga7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    public void HashSha256_RejeitaRepresentacaoNaoCanonica(string valor)
    {
        Assert.Throws<FormatException>(() => HashSha256.CriarCanonico(valor));
    }

    [Fact]
    public void FonteDocumental_RejeitaMetadadosInvalidos()
    {
        var id = FonteDocumentalId.Criar(FonteGuid);
        var versao = VersaoFonte.Criar(1);
        var hash = HashSha256.CalcularUtf8("pdf-sintetico");

        Assert.Throws<ArgumentException>(() => FonteDocumental.Criar(id, versao, hash, " ", "application/pdf", 10));
        Assert.Throws<ArgumentException>(() => FonteDocumental.Criar(id, versao, hash, "../matricula.pdf", "application/pdf", 10));
        Assert.Throws<ArgumentException>(() => FonteDocumental.Criar(id, versao, hash, "matricula.pdf", " ", 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => FonteDocumental.Criar(id, versao, hash, "matricula.pdf", "application/pdf", 0));
    }

    [Fact]
    public void FonteDocumental_NormalizaMimeType()
    {
        var fonte = CriarFonte();

        Assert.Equal("application/pdf", fonte.MimeTypeValidado);
        Assert.Equal("matricula-sintetica.pdf", fonte.NomeOriginalSanitizado);
    }

    [Fact]
    public void TrechoEvidencia_DerivaTrechoExatoEPreservaReferencias()
    {
        const string texto = "José 😀 adquiriu a fração ideal de 1/2.";
        const string trechoEsperado = "😀 adquiriu a fração ideal de 1/2";
        var inicio = texto.IndexOf(trechoEsperado, StringComparison.Ordinal);
        var fim = inicio + trechoEsperado.Length;
        var fonte = CriarFonte();

        var evidencia = TrechoEvidencia.Criar(fonte, Pagina.Criar(2), texto, inicio, fim);

        Assert.Equal(trechoEsperado, evidencia.Trecho);
        Assert.Equal(fonte.Id, evidencia.FonteId);
        Assert.Equal(fonte.Versao, evidencia.VersaoFonte);
        Assert.Equal(fonte.Hash, evidencia.HashFonte);
        Assert.Equal(Pagina.Criar(2), evidencia.Pagina);
        Assert.Equal(HashSha256.CalcularUtf8(texto), evidencia.HashTextoPagina);
        Assert.Equal(inicio, evidencia.InicioUtf16);
        Assert.Equal(fim, evidencia.FimUtf16);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(0, 0)]
    [InlineData(2, 1)]
    [InlineData(0, 4)]
    public void TrechoEvidencia_RejeitaIntervaloInvalido(int inicio, int fim)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TrechoEvidencia.Criar(CriarFonte(), Pagina.Criar(1), "abc", inicio, fim));
    }

    [Fact]
    public void TrechoEvidencia_RejeitaCorteDeParSubstituto()
    {
        const string texto = "A😀B";

        Assert.Throws<ArgumentException>(
            () => TrechoEvidencia.Criar(CriarFonte(), Pagina.Criar(1), texto, 2, 4));
        Assert.Throws<ArgumentException>(
            () => TrechoEvidencia.Criar(CriarFonte(), Pagina.Criar(1), texto, 0, 2));
    }

    private static FonteDocumental CriarFonte()
    {
        return FonteDocumental.Criar(
            FonteDocumentalId.Criar(FonteGuid),
            VersaoFonte.Criar(3),
            HashSha256.CalcularUtf8("pdf-sintetico"),
            " matricula-sintetica.pdf ",
            " Application/PDF ",
            1_024);
    }
}
