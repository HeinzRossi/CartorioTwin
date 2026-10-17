using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using CartorioTwin.Api.Seguranca;
using CartorioTwin.Application.Seguranca;
using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CartorioTwin.Api.IntegrationTests;

public sealed class AutorizacaoApiTests : IAsyncLifetime
{
    private const string Emissor = "https://idp.test";
    private const string Audiencia = "cartorio-twin-api";
    private static readonly Guid Serventia = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private readonly RSA rsa = RSA.Create(2048);
    private WebApplication? app;
    private HttpClient? client;

    public async Task InitializeAsync()
    {
        var chave = new RsaSecurityKey(rsa) { KeyId = "teste" };
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<IRepositorioVinculosAcesso>(new RepositorioVinculosFake(Serventia));
        builder.Services.AddScoped<ResolvedorAcesso>();
        builder.Services.AddSegurancaCartorioTwinParaTestes(
            new OpcoesAutenticacao
            {
                Authority = Emissor,
                Audience = Audiencia,
                WorkerClientIds = ["worker-permitido"],
            },
            options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Emissor,
                ValidateAudience = true,
                ValidAudience = Audiencia,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = chave,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
            });

        app = builder.Build();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapGet("/fontes", () => Results.Ok()).RequireAuthorization(CapacidadesAcesso.FontesLer);
        app.MapGet("/admin", () => Results.Ok()).RequireAuthorization(CapacidadesAcesso.UsuariosAdministrar);
        app.MapGet("/jobs", () => Results.Ok()).RequireAuthorization(CapacidadesAcesso.JobsProcessar);
        app.MapGet("/revisao", () => Results.Ok()).RequireAuthorization(CapacidadesAcesso.FatosRevisar);
        app.MapGet("/recursos/{serventiaId:guid}", async (
            Guid serventiaId,
            HttpContext http,
            ContextoAcessoHttp contexto) =>
        {
            var resultado = await contexto.ResolverAsync(http);
            return resultado.Contexto?.ServentiaId?.Valor == serventiaId
                ? Results.Ok()
                : Results.NotFound();
        }).RequireAuthorization(CapacidadesAcesso.FontesLer);
        app.MapPost("/selecao", async (
            SolicitacaoComServentia solicitacao,
            HttpContext http,
            ContextoAcessoHttp contexto) =>
        {
            var resultado = await contexto.ResolverAsync(http);
            return Results.Ok(new
            {
                solicitadaNoCorpo = solicitacao.ServentiaId,
                validada = resultado.Contexto!.ServentiaId!.Value.Valor,
            });
        }).RequireAuthorization(CapacidadesAcesso.FontesLer);
        await app.StartAsync();
        client = app.GetTestClient();
    }

    public async Task DisposeAsync()
    {
        client?.Dispose();
        if (app is not null)
        {
            await app.DisposeAsync();
        }

        rsa.Dispose();
    }

    [Fact]
    public async Task UsuarioDaMesmaServentia_AcessaRecurso()
    {
        Autenticar(TokenUsuario());
        client!.DefaultRequestHeaders.Add(ContextoAcessoHttp.CabecalhoServentia, Serventia.ToString("D"));

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/recursos/{Serventia:D}")).StatusCode);
    }

    [Fact]
    public async Task RecursoDeOutraServentia_FicaOcultoCom404()
    {
        Autenticar(TokenUsuario());
        client!.DefaultRequestHeaders.Add(ContextoAcessoHttp.CabecalhoServentia, Serventia.ToString("D"));

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/recursos/{Guid.NewGuid():D}")).StatusCode);
    }

    [Fact]
    public async Task ServentiaNoCorpo_NaoSubstituiContextoValidado()
    {
        Autenticar(TokenUsuario());
        client!.DefaultRequestHeaders.Add(ContextoAcessoHttp.CabecalhoServentia, Serventia.ToString("D"));
        var outra = Guid.NewGuid();

        var resposta = await client.PostAsJsonAsync("/selecao", new SolicitacaoComServentia(outra));
        var json = await resposta.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Contains(Serventia.ToString("D"), json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(outra.ToString("D"), json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AusenciaDeToken_Retorna401()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await client!.GetAsync("/fontes")).StatusCode);
    }

    [Theory]
    [InlineData("issuer")]
    [InlineData("audience")]
    [InlineData("expired")]
    [InlineData("subject")]
    [InlineData("jti")]
    [InlineData("iat")]
    [InlineData("client_id")]
    [InlineData("signature")]
    public async Task TokenInvalido_Retorna401(string defeito)
    {
        Autenticar(TokenUsuario(defeito));
        client!.DefaultRequestHeaders.Add(ContextoAcessoHttp.CabecalhoServentia, Serventia.ToString("D"));

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/fontes")).StatusCode);
    }

    [Fact]
    public async Task RoleDoToken_EIgnoradaEPoucaCapacidadeRetorna403()
    {
        Autenticar(TokenUsuario(role: "Administrador"));
        client!.DefaultRequestHeaders.Add(ContextoAcessoHttp.CabecalhoServentia, Serventia.ToString("D"));

        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/admin")).StatusCode);
    }

    [Fact]
    public async Task Worker_ExecutaJobMasNaoExecutaAcoesHumanasOuAdministrativas()
    {
        Autenticar(TokenServico("worker-permitido"));

        Assert.Equal(HttpStatusCode.OK, (await client!.GetAsync("/jobs")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/revisao")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/admin")).StatusCode);
    }

    [Fact]
    public async Task WorkerForaDaAllowlist_NaoExecutaJob()
    {
        Autenticar(TokenServico("worker-nao-permitido"));

        Assert.Equal(HttpStatusCode.Forbidden, (await client!.GetAsync("/jobs")).StatusCode);
    }

    private void Autenticar(string token) =>
        client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    private string TokenUsuario(string? defeito = null, string? role = null)
    {
        var claims = ClaimsBase("desktop-publico", defeito);
        if (defeito != "subject")
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, "usuario-externo"));
        }

        if (role is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return Emitir(claims, defeito);
    }

    private string TokenServico(string clientId)
    {
        var claims = ClaimsBase(clientId, null);
        claims.Add(new Claim("actor_type", "service"));
        claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
        return Emitir(claims, null);
    }

    private static List<Claim> ClaimsBase(string clientId, string? defeito)
    {
        var claims = new List<Claim>();
        if (defeito != "jti")
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        }

        if (defeito != "iat")
        {
            claims.Add(new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64));
        }

        if (defeito != "client_id")
        {
            claims.Add(new Claim("client_id", clientId));
        }

        return claims;
    }

    private string Emitir(List<Claim> claims, string? defeito)
    {
        using var outraRsa = defeito == "signature" ? RSA.Create(2048) : null;
        var chave = new RsaSecurityKey(outraRsa ?? rsa);
        var agora = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: defeito == "issuer" ? "https://outro.test" : Emissor,
            audience: defeito == "audience" ? "outra-api" : Audiencia,
            claims: claims,
            notBefore: defeito == "expired" ? agora.AddMinutes(-10) : agora.AddMinutes(-1),
            expires: defeito == "expired" ? agora.AddMinutes(-5) : agora.AddMinutes(5),
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.RsaSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public sealed record SolicitacaoComServentia(Guid ServentiaId);

    private sealed class RepositorioVinculosFake(Guid serventia) : IRepositorioVinculosAcesso
    {
        public Task<VinculoUsuarioAcesso?> BuscarAsync(
            string emissor,
            string subject,
            ServentiaId serventiaId,
            CancellationToken cancellationToken) => Task.FromResult<VinculoUsuarioAcesso?>(
                serventiaId.Valor == serventia && emissor == Emissor && subject == "usuario-externo"
                    ? new VinculoUsuarioAcesso(
                        UsuarioId.Criar(Guid.Parse("20000000-0000-0000-0000-000000000001")),
                        serventiaId,
                        PerfilAcesso.Operador,
                        UsuarioAtivo: true,
                        ServentiaAtiva: true,
                        VinculoAtivo: true)
                    : null);
    }
}
