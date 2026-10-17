using CartorioTwin.Application.Seguranca;
using CartorioTwin.Infrastructure.Persistencia;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace CartorioTwin.Api.Seguranca;

public static class RegistroSegurancaApi
{
    public static IServiceCollection AddSegurancaCartorioTwin(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        var opcoes = configuration.GetSection(OpcoesAutenticacao.Secao).Get<OpcoesAutenticacao>()
            ?? new OpcoesAutenticacao();
        opcoes.Validar(environment.IsEnvironment("Testing"));
        services.Configure<OpcoesAutenticacao>(configuration.GetSection(OpcoesAutenticacao.Secao));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => ConfigurarJwt(options, opcoes, environment));
        AdicionarAutorizacao(services);
        return services;
    }

    public static IServiceCollection AddSegurancaCartorioTwinParaTestes(
        this IServiceCollection services,
        OpcoesAutenticacao opcoes,
        Action<JwtBearerOptions> configurarJwt)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(opcoes);
        ArgumentNullException.ThrowIfNull(configurarJwt);
        opcoes.Validar(permitirHttp: true);

        services.AddSingleton<Microsoft.Extensions.Options.IOptions<OpcoesAutenticacao>>(
            Microsoft.Extensions.Options.Options.Create(opcoes));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            configurarJwt(options);
            ConfigurarValidacaoComum(options);
        });
        AdicionarAutorizacao(services);
        return services;
    }

    private static void ConfigurarJwt(
        JwtBearerOptions options,
        OpcoesAutenticacao opcoes,
        IHostEnvironment environment)
    {
        options.Authority = opcoes.Authority;
        options.Audience = opcoes.Audience;
        options.RequireHttpsMetadata = !environment.IsEnvironment("Testing");
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
        };
        ConfigurarValidacaoComum(options);
    }

    private static void ConfigurarValidacaoComum(JwtBearerOptions options)
    {
        options.MapInboundClaims = false;
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var principal = context.Principal!;
                var ehServico = principal.HasClaim("actor_type", "service");
                var clientId = principal.FindFirst("client_id")?.Value
                    ?? principal.FindFirst("azp")?.Value;
                var subject = principal.FindFirst("sub")?.Value;
                var possuiBase = principal.HasClaim(x => x.Type == "iss")
                    && principal.HasClaim(x => x.Type == "jti")
                    && principal.HasClaim(x => x.Type == "iat")
                    && !string.IsNullOrWhiteSpace(clientId);
                if (!possuiBase || (!ehServico && string.IsNullOrWhiteSpace(subject)))
                {
                    context.Fail("O token não contém as claims obrigatórias.");
                }

                return Task.CompletedTask;
            },
        };
    }

    private static void AdicionarAutorizacao(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ContextoAcessoHttp>();
        services.AddScoped<IAuthorizationHandler, ManipuladorCapacidade>();
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
            .AdicionarPolitica(CapacidadesAcesso.FontesLer)
            .AdicionarPolitica(CapacidadesAcesso.FontesCriar)
            .AdicionarPolitica(CapacidadesAcesso.FatosRevisar)
            .AdicionarPolitica(CapacidadesAcesso.SnapshotsRevisar)
            .AdicionarPolitica(CapacidadesAcesso.SimulacoesExecutar)
            .AdicionarPolitica(CapacidadesAcesso.RevisoesJuridicasRegistrar)
            .AdicionarPolitica(CapacidadesAcesso.DossiesExportar)
            .AdicionarPolitica(CapacidadesAcesso.UsuariosAdministrar)
            .AdicionarPolitica(CapacidadesAcesso.AuditoriaConsultar)
            .AdicionarPolitica(CapacidadesAcesso.JobsProcessar);
    }

    private static AuthorizationBuilder AdicionarPolitica(
        this AuthorizationBuilder builder,
        string capacidade) => builder.AddPolicy(
            capacidade,
            policy => policy
                .RequireAuthenticatedUser()
                .AddRequirements(new RequisitoCapacidade(capacidade)));
}
