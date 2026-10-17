using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;

namespace CartorioTwin.Desktop.Client.Autenticacao;

public sealed record ConfiguracaoOidcDesktop
{
    private ConfiguracaoOidcDesktop(Uri authority, string clientId, string scope, Uri redirectUri)
    {
        Authority = authority;
        ClientId = clientId;
        Scope = scope;
        RedirectUri = redirectUri;
    }

    public Uri Authority { get; }
    public string ClientId { get; }
    public string Scope { get; }
    public Uri RedirectUri { get; }

    public static ConfiguracaoOidcDesktop Criar(
        Uri authority,
        string clientId,
        IEnumerable<string> scopes,
        Uri redirectUri)
    {
        ArgumentNullException.ThrowIfNull(authority);
        ArgumentNullException.ThrowIfNull(scopes);
        ArgumentNullException.ThrowIfNull(redirectUri);
        if (!authority.IsAbsoluteUri || authority.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("A authority deve ser HTTPS absoluta.", nameof(authority));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        if (!redirectUri.IsAbsoluteUri)
        {
            throw new ArgumentException("A redirect URI deve ser absoluta.", nameof(redirectUri));
        }

        var valores = scopes.Select(x => x?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Cast<string>()
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        if (!valores.Contains("openid", StringComparer.Ordinal))
        {
            throw new ArgumentException("O scope openid é obrigatório.", nameof(scopes));
        }

        return new ConfiguracaoOidcDesktop(authority, clientId.Trim(), string.Join(' ', valores), redirectUri);
    }
}

public sealed record ResultadoNavegadorOidc(bool Sucesso, string? Resposta, bool Cancelado = false);

public interface INavegadorSistemaOidc
{
    Task<ResultadoNavegadorOidc> AbrirAsync(
        Uri inicio,
        Uri retorno,
        CancellationToken cancellationToken);
}

public interface IArmazenamentoRefreshToken
{
    Task SalvarAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<string?> LerAsync(CancellationToken cancellationToken = default);
    Task LimparAsync(CancellationToken cancellationToken = default);
}

public sealed class SessaoOidcDesktop
{
    private readonly OidcClient cliente;
    private readonly IArmazenamentoRefreshToken armazenamentoRefreshToken;

    public SessaoOidcDesktop(
        ConfiguracaoOidcDesktop configuracao,
        INavegadorSistemaOidc navegador,
        IArmazenamentoRefreshToken armazenamentoRefreshToken)
    {
        ArgumentNullException.ThrowIfNull(configuracao);
        ArgumentNullException.ThrowIfNull(navegador);
        this.armazenamentoRefreshToken = armazenamentoRefreshToken
            ?? throw new ArgumentNullException(nameof(armazenamentoRefreshToken));
        Opcoes = CriarOpcoes(configuracao, navegador);
        cliente = new OidcClient(Opcoes);
    }

    public OidcClientOptions Opcoes { get; }
    public string? AccessToken { get; private set; }
    public DateTimeOffset? AccessTokenExpiraEm { get; private set; }

    public async Task LoginAsync(CancellationToken cancellationToken = default)
    {
        var resultado = await cliente.LoginAsync(new LoginRequest(), cancellationToken);
        if (resultado.IsError)
        {
            throw new InvalidOperationException($"Falha OIDC: {resultado.Error}.");
        }

        AccessToken = resultado.AccessToken;
        AccessTokenExpiraEm = resultado.AccessTokenExpiration;
        if (!string.IsNullOrWhiteSpace(resultado.RefreshToken))
        {
            await armazenamentoRefreshToken.SalvarAsync(resultado.RefreshToken, cancellationToken);
        }
    }

    public async Task RenovarAsync(CancellationToken cancellationToken = default)
    {
        var refreshToken = await armazenamentoRefreshToken.LerAsync(cancellationToken)
            ?? throw new InvalidOperationException("Não há refresh token protegido disponível.");
        var resultado = await cliente.RefreshTokenAsync(refreshToken, cancellationToken: cancellationToken);
        if (resultado.IsError)
        {
            throw new InvalidOperationException($"Falha ao renovar sessão OIDC: {resultado.Error}.");
        }

        AccessToken = resultado.AccessToken;
        AccessTokenExpiraEm = DateTimeOffset.UtcNow.AddSeconds(resultado.ExpiresIn);
        if (!string.IsNullOrWhiteSpace(resultado.RefreshToken))
        {
            await armazenamentoRefreshToken.SalvarAsync(resultado.RefreshToken, cancellationToken);
        }
    }

    public async Task LogoutLocalAsync(CancellationToken cancellationToken = default)
    {
        AccessToken = null;
        AccessTokenExpiraEm = null;
        await armazenamentoRefreshToken.LimparAsync(cancellationToken);
    }

    public static OidcClientOptions CriarOpcoes(
        ConfiguracaoOidcDesktop configuracao,
        INavegadorSistemaOidc navegador)
    {
        ArgumentNullException.ThrowIfNull(configuracao);
        ArgumentNullException.ThrowIfNull(navegador);
        return new OidcClientOptions
        {
            Authority = configuracao.Authority.AbsoluteUri.TrimEnd('/'),
            ClientId = configuracao.ClientId,
            ClientSecret = null,
            Scope = configuracao.Scope,
            RedirectUri = configuracao.RedirectUri.AbsoluteUri,
            Browser = new AdaptadorNavegador(navegador),
            LoadProfile = false,
        };
    }

    private sealed class AdaptadorNavegador(INavegadorSistemaOidc navegador) : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(
            BrowserOptions options,
            CancellationToken cancellationToken = default)
        {
            var resultado = await navegador.AbrirAsync(
                new Uri(options.StartUrl, UriKind.Absolute),
                new Uri(options.EndUrl, UriKind.Absolute),
                cancellationToken);
            return new BrowserResult
            {
                ResultType = resultado.Sucesso
                    ? BrowserResultType.Success
                    : resultado.Cancelado ? BrowserResultType.UserCancel : BrowserResultType.UnknownError,
                Response = resultado.Resposta,
            };
        }
    }
}
