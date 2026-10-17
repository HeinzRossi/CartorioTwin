using CartorioTwin.Infrastructure.Persistencia;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CartorioTwin.Infrastructure.Tests;

[Collection(SqlServerCollection.Name)]
public sealed class PersistenciaSqlServerTests(SqlServerContainerFixture fixture)
{
    private const string Hash = "sha256:aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [SqlServerFact]
    public async Task Migration_PersisteGrafoAposReaberturaEReinicioDoSqlServer()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            AdicionarGrafoDocumental(context);
            await context.SaveChangesAsync();
        }

        await fixture.Container.StopAsync();
        await fixture.Container.StartAsync();

        await using (var context = CriarContexto(database))
        {
            Assert.Equal(1, await context.Set<FonteDocumentalRegistro>().CountAsync());
            Assert.Equal(1, await context.Set<EvidenciaFonteRegistro>().CountAsync());
            Assert.Equal(1, await context.Set<CampoCandidatoEvidenciaRegistro>().CountAsync());
            Assert.Equal(1, await context.Set<RevisaoCampoRegistro>().CountAsync());
            Assert.Equal(1, await context.Set<TentativaJobRegistro>().CountAsync());
            Assert.Equal(1, await context.Set<AutorizacaoRevisaoRegistro>().CountAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task FkComposta_RejeitaReferenciaEntreServentias()
    {
        var database = NomeBanco();
        await using var context = CriarContexto(database);
        await context.Database.MigrateAsync();

        var serventiaA = Guid.NewGuid();
        var serventiaB = Guid.NewGuid();
        var operacao = Guid.NewGuid();
        context.AddRange(
            Serventia(serventiaA, "SP"),
            Serventia(serventiaB, "RJ"),
            Operacao(serventiaA, operacao),
            Fonte(serventiaB, operacao, Guid.NewGuid()));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task Constraints_RejeitamPaginaDuplicadaHashEOffsetsInvalidos()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventia = Guid.NewGuid();
            var operacao = Guid.NewGuid();
            var fonte = Guid.NewGuid();
            context.AddRange(Serventia(serventia, "SP"), Operacao(serventia, operacao), Fonte(serventia, operacao, fonte));
            context.AddRange(Pagina(serventia, operacao, fonte, 1), Pagina(serventia, operacao, fonte, 1));
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task Constraint_RejeitaHashForaDoFormatoCanonico()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventia = Guid.NewGuid();
            var operacao = Guid.NewGuid();
            var fonte = Fonte(serventia, operacao, Guid.NewGuid());
            fonte.Sha256 = "SHA256:INVALIDO";
            context.AddRange(Serventia(serventia, "SP"), Operacao(serventia, operacao), fonte);
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task Constraint_RejeitaIntervaloDeEvidenciaVazio()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            AdicionarGrafoDocumental(context);
            var evidencia = context.ChangeTracker.Entries<EvidenciaFonteRegistro>().Single().Entity;
            evidencia.InicioUtf16 = 5;
            evidencia.FimUtf16 = 5;
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task RowVersion_DetectaAtualizacaoConcorrente()
    {
        var database = NomeBanco();
        var serventia = Guid.NewGuid();
        var operacao = Guid.NewGuid();
        await using (var setup = CriarContexto(database))
        {
            await setup.Database.MigrateAsync();
            setup.AddRange(Serventia(serventia, "SP"), Operacao(serventia, operacao));
            await setup.SaveChangesAsync();
        }

        await using var primeiro = CriarContexto(database);
        await using var segundo = CriarContexto(database);
        var a = await primeiro.Set<OperacaoRegistro>().SingleAsync();
        var b = await segundo.Set<OperacaoRegistro>().SingleAsync();
        a.EstadoWorkflow = "revisao";
        b.EstadoWorkflow = "contestado";
        await primeiro.SaveChangesAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => segundo.SaveChangesAsync());
        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task Migration_GeraScriptIdempotente()
    {
        var database = NomeBanco();
        await using var context = CriarContexto(database);
        var migrator = context.GetService<IMigrator>();
        var script = migrator.GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);

        Assert.Contains("S1T07CatalogoInicial", script, StringComparison.Ordinal);
        Assert.Contains("S1T08IdentidadeAutorizacao", script, StringComparison.Ordinal);
        Assert.Contains("IF NOT EXISTS", script, StringComparison.OrdinalIgnoreCase);
    }

    [SqlServerFact]
    public async Task IdentidadeExterna_EUnicaPorEmissorESubject()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventia = Guid.NewGuid();
            context.AddRange(
                Serventia(serventia, "SP"),
                Usuario(Guid.NewGuid(), "https://idp.test", "mesmo-sub"),
                Usuario(Guid.NewGuid(), "https://idp.test", "mesmo-sub"));

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task UsuarioServentia_RejeitaPerfilDesconhecido()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventia = Guid.NewGuid();
            var usuario = Guid.NewGuid();
            context.AddRange(
                Serventia(serventia, "SP"),
                Usuario(usuario, "https://idp.test", "usuario-perfil"),
                new UsuarioServentiaRegistro
                {
                    UsuarioId = usuario,
                    ServentiaId = serventia,
                    Perfil = "SuperAdministrador",
                    Ativo = true,
                });

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task AutorizacaoRevisao_RejeitaReferenciaCruzadaEntreServentias()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventiaA = Guid.NewGuid();
            var serventiaB = Guid.NewGuid();
            var operacaoA = Guid.NewGuid();
            var usuario = Guid.NewGuid();
            context.AddRange(
                Serventia(serventiaA, "SP"),
                Serventia(serventiaB, "RJ"),
                Operacao(serventiaA, operacaoA),
                Usuario(usuario, "https://idp.test", "usuario-1"),
                Autorizacao(serventiaB, operacaoA, usuario));

            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    [SqlServerFact]
    public async Task AutorizacaoRevisao_EAppendOnlyNoDbContext()
    {
        var database = NomeBanco();
        await using (var context = CriarContexto(database))
        {
            await context.Database.MigrateAsync();
            var serventia = Guid.NewGuid();
            var operacao = Guid.NewGuid();
            var usuario = Guid.NewGuid();
            context.AddRange(
                Serventia(serventia, "SP"),
                Operacao(serventia, operacao),
                Usuario(usuario, "https://idp.test", "usuario-append-only"),
                new UsuarioServentiaRegistro { UsuarioId = usuario, ServentiaId = serventia, Perfil = "Conferente", Ativo = true },
                Autorizacao(serventia, operacao, usuario));
            await context.SaveChangesAsync();
        }

        await using (var context = CriarContexto(database))
        {
            var autorizacao = await context.Set<AutorizacaoRevisaoRegistro>().SingleAsync();
            autorizacao.Capacidade = "alterada";
            await Assert.ThrowsAsync<InvalidOperationException>(() => context.SaveChangesAsync());
        }

        await ExcluirBanco(database);
    }

    private CartorioTwinDbContext CriarContexto(string database)
    {
        var builder = new SqlConnectionStringBuilder(fixture.Container.GetConnectionString())
        {
            InitialCatalog = database,
            TrustServerCertificate = true,
        };
        var options = new DbContextOptionsBuilder<CartorioTwinDbContext>()
            .UseSqlServer(builder.ConnectionString)
            .Options;
        return new CartorioTwinDbContext(options);
    }

    private async Task ExcluirBanco(string database)
    {
        await using var context = CriarContexto(database);
        await context.Database.EnsureDeletedAsync();
    }

    private static string NomeBanco() => $"CartorioTwin_{Guid.NewGuid():N}";

    private static ServentiaRegistro Serventia(Guid id, string uf) => new()
    {
        Id = id,
        Uf = uf,
        Nome = $"Serventia {uf}",
        Ativa = true,
        CriadaEmUtc = DateTimeOffset.UtcNow,
    };

    private static OperacaoRegistro Operacao(Guid serventia, Guid id) => new()
    {
        Id = id,
        ServentiaId = serventia,
        MatriculaId = Guid.NewGuid(),
        RevisaoAtual = 0,
        EstadoWorkflow = "criada",
        CriadaEmUtc = DateTimeOffset.UtcNow,
        AtualizadaEmUtc = DateTimeOffset.UtcNow,
    };

    private static UsuarioRegistro Usuario(Guid id, string emissor, string subject) => new()
    {
        Id = id,
        EmissorIdentidade = emissor,
        IdentidadeExterna = subject,
        NomeExibicao = "Usuário sintético",
        Ativo = true,
    };

    private static AutorizacaoRevisaoRegistro Autorizacao(Guid serventia, Guid operacao, Guid usuario) => new()
    {
        Id = Guid.NewGuid(),
        ServentiaId = serventia,
        OperacaoId = operacao,
        TipoRecurso = "FatoDocumental",
        RecursoId = Guid.NewGuid(),
        VersaoAlvo = 1,
        UsuarioId = usuario,
        PerfilEfetivo = "Conferente",
        Capacidade = "fatos.revisar",
        EmitidaEmUtc = DateTimeOffset.UtcNow,
        CorrelationId = Guid.NewGuid(),
    };

    private static FonteDocumentalRegistro Fonte(Guid serventia, Guid operacao, Guid id) => new()
    {
        Id = id,
        ServentiaId = serventia,
        OperacaoId = operacao,
        Versao = 1,
        Sha256 = Hash,
        StorageKey = $"fontes/{id:N}",
        MimeValidado = "application/pdf",
        Bytes = 100,
        EstadoTecnico = "liberada",
        CriadaEmUtc = DateTimeOffset.UtcNow,
    };

    private static PaginaFonteRegistro Pagina(Guid serventia, Guid operacao, Guid fonte, int numero) => new()
    {
        Id = Guid.NewGuid(),
        ServentiaId = serventia,
        OperacaoId = operacao,
        FonteId = fonte,
        Numero = numero,
        Rotacao = 0,
    };

    private static void AdicionarGrafoDocumental(CartorioTwinDbContext context)
    {
        var serventia = Guid.NewGuid();
        var operacao = Guid.NewGuid();
        var fonte = Guid.NewGuid();
        var pagina = Guid.NewGuid();
        var extracao = Guid.NewGuid();
        var paginaExtraida = Guid.NewGuid();
        var evidencia = Guid.NewGuid();
        var segmento = Guid.NewGuid();
        var interpretacao = Guid.NewGuid();
        var ato = Guid.NewGuid();
        var campo = Guid.NewGuid();
        var revisao = Guid.NewGuid();
        var job = Guid.NewGuid();
        var usuario = Guid.NewGuid();

        context.AddRange(
            Serventia(serventia, "SP"),
            Operacao(serventia, operacao),
            Usuario(usuario, "https://idp.test", $"usuario-{usuario:N}"),
            new UsuarioServentiaRegistro { UsuarioId = usuario, ServentiaId = serventia, Perfil = "Conferente", Ativo = true },
            Autorizacao(serventia, operacao, usuario),
            Fonte(serventia, operacao, fonte),
            new PaginaFonteRegistro { Id = pagina, ServentiaId = serventia, OperacaoId = operacao, FonteId = fonte, Numero = 1 },
            new ExecucaoExtracaoRegistro { Id = extracao, ServentiaId = serventia, OperacaoId = operacao, FonteId = fonte, RevisaoConfiguracao = 1, Estado = "concluida", CriadaEmUtc = DateTimeOffset.UtcNow },
            new PaginaExtraidaRegistro { Id = paginaExtraida, ServentiaId = serventia, OperacaoId = operacao, ExtracaoId = extracao, PaginaFonteId = pagina, Estrategia = "texto", TextoStorageKey = "textos/1", TextoHash = Hash },
            new EvidenciaFonteRegistro { Id = evidencia, ServentiaId = serventia, OperacaoId = operacao, ExtracaoId = extracao, PaginaExtraidaId = paginaExtraida, InicioUtf16 = 0, FimUtf16 = 5, Trecho = "teste" },
            new SegmentoAtoRegistro { Id = segmento, ServentiaId = serventia, OperacaoId = operacao, ExtracaoId = extracao, OrdemNaFonte = 1 },
            new SegmentoAtoEvidenciaRegistro { ServentiaId = serventia, OperacaoId = operacao, SegmentoId = segmento, EvidenciaId = evidencia },
            new ExecucaoInterpretacaoRegistro { Id = interpretacao, ServentiaId = serventia, OperacaoId = operacao, SegmentoId = segmento, ProvedorVersao = "sintetico", PromptHash = Hash, SchemaVersao = "1", ConfiguracaoHash = Hash, SaidaStorageKey = "interpretacoes/1" },
            new AtoCandidatoRegistro { Id = ato, ServentiaId = serventia, OperacaoId = operacao, MatriculaId = Guid.NewGuid(), InterpretacaoId = interpretacao, TipoSugerido = "nao_identificado", OrdemDocumental = 1 },
            new CampoCandidatoRegistro { Id = campo, ServentiaId = serventia, OperacaoId = operacao, AtoCandidatoId = ato, CaminhoCampo = "ato.tipo", ValorSugeridoJson = "\"teste\"", Tipo = "texto", Estado = "confirmado" },
            new CampoCandidatoEvidenciaRegistro { ServentiaId = serventia, OperacaoId = operacao, CampoId = campo, EvidenciaId = evidencia },
            new RevisaoCampoRegistro { Id = revisao, ServentiaId = serventia, OperacaoId = operacao, CampoId = campo, NumeroVersao = 1, ValorFinalJson = "\"teste\"", Decisao = "confirmar", UsuarioId = Guid.NewGuid(), AutorizacaoId = Guid.NewGuid(), EmUtc = DateTimeOffset.UtcNow },
            new JobProcessamentoRegistro { Id = job, ServentiaId = serventia, OperacaoId = operacao, FonteId = fonte, Tipo = "extracao", ChaveIdempotencia = Guid.NewGuid().ToString("N"), ManifestoHash = Hash, Estado = "concluido", CriadaEmUtc = DateTimeOffset.UtcNow },
            new TentativaJobRegistro { Id = Guid.NewGuid(), ServentiaId = serventia, OperacaoId = operacao, JobId = job, Numero = 1, InicioUtc = DateTimeOffset.UtcNow, Estado = "concluida" });
    }
}
