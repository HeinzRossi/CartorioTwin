using CartorioTwin.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace CartorioTwin.Infrastructure.Tests;

public sealed class ModeloPersistenciaTests
{
    [Fact]
    public void Modelo_ContemCatalogoCompletoEChavesDeContexto()
    {
        using var context = CriarContextoModelo();
        var model = context.Model;

        Assert.Equal(47, model.GetEntityTypes().Count());
        Assert.Contains(model.GetEntityTypes(), x => x.GetTableName() == "EvidenciaFonte");
        Assert.Contains(model.GetEntityTypes(), x => x.GetTableName() == "CampoCandidatoEvidencia");
        Assert.Contains(model.GetEntityTypes(), x => x.GetTableName() == "JobProcessamento");
        Assert.Contains(model.GetEntityTypes(), x => x.GetTableName() == "AutorizacaoRevisao");

        var pagina = model.FindEntityType(typeof(PaginaFonteRegistro));
        Assert.NotNull(pagina);
        Assert.Contains(pagina.GetForeignKeys(), fk =>
            fk.Properties.Select(x => x.Name).SequenceEqual(["ServentiaId", "OperacaoId", "FonteId"]));

        var join = model.FindEntityType(typeof(CampoCandidatoEvidenciaRegistro));
        Assert.NotNull(join);
        Assert.Equal(4, join.FindPrimaryKey()!.Properties.Count);
    }

    [Fact]
    public void Domain_ContinuaSemPacotesOuReferenciasDeProjeto()
    {
        var domainProject = File.ReadAllText(LocalizarRaiz("src", "CartorioTwin.Domain", "CartorioTwin.Domain.csproj"));

        Assert.DoesNotContain("PackageReference", domainProject, StringComparison.Ordinal);
        Assert.DoesNotContain("ProjectReference", domainProject, StringComparison.Ordinal);
    }

    private static CartorioTwinDbContext CriarContextoModelo()
    {
        var options = new DbContextOptionsBuilder<CartorioTwinDbContext>()
            .UseSqlServer("Server=localhost;Database=ModelOnly;Integrated Security=true;TrustServerCertificate=True")
            .Options;
        return new CartorioTwinDbContext(options);
    }

    private static string LocalizarRaiz(params string[] partes)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "CartorioTwin.sln")))
        {
            directory = directory.Parent;
        }

        Assert.NotNull(directory);
        return Path.Combine([directory.FullName, .. partes]);
    }
}
