using CartorioTwin.Api.Seguranca;

namespace CartorioTwin.Api.IntegrationTests;

public sealed class OpcoesAutenticacaoTests
{
    [Fact]
    public void ConfiguracaoDeProducao_ExigeHttpsAudienceEAllowlistDeWorker()
    {
        Assert.Throws<InvalidOperationException>(() => new OpcoesAutenticacao().Validar(permitirHttp: false));
        Assert.Throws<InvalidOperationException>(() => new OpcoesAutenticacao
        {
            Authority = "http://idp.test",
            Audience = "api",
            WorkerClientIds = ["worker"],
        }.Validar(permitirHttp: false));
        Assert.Throws<ArgumentException>(() => new OpcoesAutenticacao
        {
            Authority = "https://idp.test",
            WorkerClientIds = ["worker"],
        }.Validar(permitirHttp: false));
        Assert.Throws<InvalidOperationException>(() => new OpcoesAutenticacao
        {
            Authority = "https://idp.test",
            Audience = "api",
            WorkerClientIds = [],
        }.Validar(permitirHttp: false));
    }
}
