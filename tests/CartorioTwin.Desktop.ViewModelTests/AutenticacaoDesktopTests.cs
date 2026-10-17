using System.Text;
using CartorioTwin.Desktop.Client.Autenticacao;
using CartorioTwin.Desktop.Wpf.Autenticacao;

namespace CartorioTwin.Desktop.ViewModelTests;

public sealed class AutenticacaoDesktopTests
{
    [Fact]
    public void Configuracao_PreparaClientePublicoComPkceSemSegredo()
    {
        var configuracao = Configuracao();

        var opcoes = SessaoOidcDesktop.CriarOpcoes(configuracao, new NavegadorFake());

        Assert.Null(opcoes.ClientSecret);
        Assert.Equal("cartorio-twin-desktop", opcoes.ClientId);
        Assert.Contains("openid", opcoes.Scope, StringComparison.Ordinal);
        Assert.NotNull(opcoes.Browser);
    }

    [Fact]
    public void Configuracao_RejeitaAuthorityInseguraEScopeSemOpenId()
    {
        Assert.Throws<ArgumentException>(() => ConfiguracaoOidcDesktop.Criar(
            new Uri("http://idp.test"),
            "cliente",
            ["openid"],
            new Uri("cartoriotwin://callback")));
        Assert.Throws<ArgumentException>(() => ConfiguracaoOidcDesktop.Criar(
            new Uri("https://idp.test"),
            "cliente",
            ["profile"],
            new Uri("cartoriotwin://callback")));
    }

    [Fact]
    public async Task RefreshToken_FazRoundTripDpapiSemTextoClaroELimpaNoLogout()
    {
        var diretorio = Path.Combine(Path.GetTempPath(), $"CartorioTwin-{Guid.NewGuid():N}");
        try
        {
            var armazenamento = new ArmazenamentoRefreshTokenDpapi(diretorio);
            const string token = "refresh-token-sintetico-confidencial";
            await armazenamento.SalvarAsync(token);

            var bytes = await File.ReadAllBytesAsync(Path.Combine(diretorio, "refresh-token.bin"));
            Assert.DoesNotContain(token, Encoding.UTF8.GetString(bytes), StringComparison.Ordinal);
            Assert.Equal(token, await armazenamento.LerAsync());

            var sessao = new SessaoOidcDesktop(Configuracao(), new NavegadorFake(), armazenamento);
            await sessao.LogoutLocalAsync();

            Assert.Null(sessao.AccessToken);
            Assert.Null(await armazenamento.LerAsync());
        }
        finally
        {
            if (Directory.Exists(diretorio))
            {
                Directory.Delete(diretorio, recursive: true);
            }
        }
    }

    private static ConfiguracaoOidcDesktop Configuracao() => ConfiguracaoOidcDesktop.Criar(
        new Uri("https://idp.test"),
        "cartorio-twin-desktop",
        ["openid", "profile", "offline_access"],
        new Uri("cartoriotwin://callback"));

    private sealed class NavegadorFake : INavegadorSistemaOidc
    {
        public Task<ResultadoNavegadorOidc> AbrirAsync(
            Uri inicio,
            Uri retorno,
            CancellationToken cancellationToken) =>
            Task.FromResult(new ResultadoNavegadorOidc(false, null, Cancelado: true));
    }
}
