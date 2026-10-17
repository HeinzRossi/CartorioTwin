namespace CartorioTwin.Api.Seguranca;

public sealed class OpcoesAutenticacao
{
    public const string Secao = "Autenticacao:Oidc";

    public string Authority { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string[] WorkerClientIds { get; init; } = [];

    public void Validar(bool permitirHttp)
    {
        if (!Uri.TryCreate(Authority, UriKind.Absolute, out var authority)
            || (!permitirHttp && authority.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException("Autenticacao:Oidc:Authority deve ser uma URI HTTPS absoluta.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(Audience);
        if (WorkerClientIds is null
            || WorkerClientIds.Length == 0
            || WorkerClientIds.Any(string.IsNullOrWhiteSpace)
            || WorkerClientIds.Distinct(StringComparer.Ordinal).Count() != WorkerClientIds.Length)
        {
            throw new InvalidOperationException("WorkerClientIds deve conter ao menos um identificador autorizado, único e não vazio.");
        }
    }
}
