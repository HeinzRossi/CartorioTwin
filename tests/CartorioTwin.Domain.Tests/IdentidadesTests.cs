using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Domain.Tests;

public sealed class IdentidadesTests
{
    [Fact]
    public void Fabricas_RejeitamIdentificadorVazio()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PessoaId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => ImovelId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => MatriculaId.Criar(Guid.Empty));
        Assert.Throws<ArgumentOutOfRangeException>(() => ServentiaId.Criar(Guid.Empty));
    }

    [Fact]
    public void Identificadores_IguaisDependemSomenteDoUuid()
    {
        var valor = Guid.Parse("18da0498-2eb1-48dc-9c2c-a2024e3e2274");

        Assert.Equal(PessoaId.Criar(valor), PessoaId.Criar(valor));
        Assert.Equal(ImovelId.Criar(valor), ImovelId.Criar(valor));
        Assert.Equal(MatriculaId.Criar(valor), MatriculaId.Criar(valor));
        Assert.Equal(ServentiaId.Criar(valor), ServentiaId.Criar(valor));
    }

    [Fact]
    public void Identificadores_DiferentesNaoSaoIguais()
    {
        var primeiro = Guid.Parse("18da0498-2eb1-48dc-9c2c-a2024e3e2274");
        var segundo = Guid.Parse("4d56104c-687d-4f30-8cf5-d127bc0e27f1");

        Assert.NotEqual(PessoaId.Criar(primeiro), PessoaId.Criar(segundo));
        Assert.NotEqual(ImovelId.Criar(primeiro), ImovelId.Criar(segundo));
        Assert.NotEqual(MatriculaId.Criar(primeiro), MatriculaId.Criar(segundo));
        Assert.NotEqual(ServentiaId.Criar(primeiro), ServentiaId.Criar(segundo));
    }

    [Fact]
    public void ToString_UsaFormatoUuidCanonico()
    {
        const string uuid = "18da0498-2eb1-48dc-9c2c-a2024e3e2274";

        Assert.Equal(uuid, PessoaId.Criar(Guid.Parse(uuid)).ToString());
    }
}
