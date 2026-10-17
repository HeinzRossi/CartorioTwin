using System.Security.Claims;
using CartorioTwin.Application.Seguranca;
using CartorioTwin.Domain.Identidades;
using Microsoft.Extensions.Options;

namespace CartorioTwin.Api.Seguranca;

public sealed class ContextoAcessoHttp(
    ResolvedorAcesso resolvedor,
    IOptions<OpcoesAutenticacao> opcoes)
{
    public const string CabecalhoServentia = "X-CartorioTwin-Serventia";
    private const string ItemContexto = "CartorioTwin.ContextoAcesso";

    public async Task<ResultadoResolucaoAcesso> ResolverAsync(
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        if (httpContext.Items.TryGetValue(ItemContexto, out var armazenado)
            && armazenado is ResultadoResolucaoAcesso resultadoArmazenado)
        {
            return resultadoArmazenado;
        }

        var principal = httpContext.User;
        ResultadoResolucaoAcesso resultado;
        if (EhServico(principal, out var clientId))
        {
            var autorizado = opcoes.Value.WorkerClientIds.Contains(clientId, StringComparer.Ordinal);
            resultado = new(
                ContextoAcesso.Servico(clientId, autorizado),
                FalhaResolucaoAcesso.Nenhuma);
        }
        else
        {
            var emissor = principal.FindFirstValue("iss");
            var subject = principal.FindFirstValue("sub");
            var valorServentia = httpContext.Request.Headers[CabecalhoServentia].ToString();
            if (!Guid.TryParseExact(valorServentia, "D", out var serventiaGuid) || serventiaGuid == Guid.Empty)
            {
                resultado = new(null, FalhaResolucaoAcesso.IdentidadeInvalida);
            }
            else
            {
                resultado = await resolvedor.ResolverUsuarioAsync(
                    emissor ?? string.Empty,
                    subject ?? string.Empty,
                    ServentiaId.Criar(serventiaGuid),
                    cancellationToken);
            }
        }

        httpContext.Items[ItemContexto] = resultado;
        return resultado;
    }

    private static bool EhServico(ClaimsPrincipal principal, out string clientId)
    {
        clientId = principal.FindFirstValue("client_id")
            ?? principal.FindFirstValue("azp")
            ?? string.Empty;
        return principal.HasClaim("actor_type", "service") && !string.IsNullOrWhiteSpace(clientId);
    }
}
