using CartorioTwin.Application.Seguranca;
using Microsoft.AspNetCore.Authorization;

namespace CartorioTwin.Api.Seguranca;

public sealed record RequisitoCapacidade(string Capacidade) : IAuthorizationRequirement;

public sealed class ManipuladorCapacidade(ContextoAcessoHttp contextoAcesso)
    : AuthorizationHandler<RequisitoCapacidade>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequisitoCapacidade requirement)
    {
        if (context.Resource is not HttpContext httpContext || context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var resultado = await contextoAcesso.ResolverAsync(httpContext, httpContext.RequestAborted);
        if (resultado.Contexto?.Possui(requirement.Capacidade) == true)
        {
            context.Succeed(requirement);
        }
    }
}
