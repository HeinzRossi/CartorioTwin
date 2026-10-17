using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CartorioTwin.Infrastructure.Persistencia;

internal static class ConfiguracaoCatalogo
{
    private static readonly Type[] TiposCatalogo =
    [
        typeof(ServentiaRegistro), typeof(UsuarioRegistro), typeof(UsuarioServentiaRegistro),
        typeof(AutorizacaoRevisaoRegistro),
        typeof(OperacaoRegistro), typeof(MatriculaReferenciaRegistro), typeof(FonteDocumentalRegistro),
        typeof(PaginaFonteRegistro), typeof(ExecucaoExtracaoRegistro), typeof(PaginaExtraidaRegistro),
        typeof(EvidenciaFonteRegistro), typeof(SegmentoAtoRegistro), typeof(SegmentoAtoEvidenciaRegistro),
        typeof(ExecucaoInterpretacaoRegistro), typeof(AtoCandidatoRegistro), typeof(CampoCandidatoRegistro),
        typeof(CampoCandidatoEvidenciaRegistro), typeof(RevisaoCampoRegistro), typeof(RevisaoAtoRegistro),
        typeof(RevisaoAtoCampoRegistro), typeof(VinculoAtosFontesRegistro), typeof(EventoRevisadoRegistro),
        typeof(EventoRevisadoAlvoRegistro), typeof(ExecucaoReconstrucaoRegistro), typeof(ReconstrucaoEventoRegistro),
        typeof(LacunaReconstrucaoRegistro), typeof(MatriculaSnapshotRegistro), typeof(SnapshotFonteRegistro),
        typeof(TitularidadeSnapshotRegistro), typeof(TitularidadeEvidenciaRegistro), typeof(AprovacaoSnapshotRegistro),
        typeof(DireitoDocumentadoRegistro), typeof(DireitoTitularRegistro), typeof(DireitoEvidenciaRegistro),
        typeof(CenarioRegistro), typeof(CenarioRevisaoRegistro), typeof(TransmissaoPropostaRegistro),
        typeof(ExecucaoSimulacaoRegistro), typeof(ValidadeExecucaoRegistro), typeof(AchadoRegistro),
        typeof(AchadoEvidenciaRegistro), typeof(RevisaoAchadoRegistro), typeof(DossieEmitidoRegistro),
        typeof(PacoteRegraRegistro), typeof(JobProcessamentoRegistro), typeof(TentativaJobRegistro),
        typeof(EventoAuditoriaRegistro),
    ];

    public static void AplicarCatalogoCartorioTwin(this ModelBuilder modelBuilder)
    {
        foreach (var type in TiposCatalogo)
        {
            modelBuilder.Entity(type).ToTable(type.Name[..^"Registro".Length]);
        }

        ConfigurarChaves(modelBuilder);
        ConfigurarRelacionamentos(modelBuilder);
        ConfigurarRestricoes(modelBuilder);
        ConfigurarTipos(modelBuilder);
    }

    private static void ConfigurarChaves(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServentiaRegistro>().HasKey(x => x.Id);
        modelBuilder.Entity<UsuarioRegistro>().HasKey(x => x.Id);
        modelBuilder.Entity<UsuarioServentiaRegistro>().HasKey(x => new { x.UsuarioId, x.ServentiaId });

        foreach (var type in TiposCatalogo.Where(t => typeof(IRegistroTenant).IsAssignableFrom(t)))
        {
            var entity = modelBuilder.Entity(type);
            entity.HasKey(nameof(IRegistroTenant.Id));
            entity.HasAlternateKey(nameof(IRegistroTenant.ServentiaId), nameof(IRegistroTenant.Id));
            entity.Property(nameof(IRegistroTenant.Id)).ValueGeneratedNever();
            entity.HasIndex(nameof(IRegistroTenant.ServentiaId));

            if (typeof(IRegistroOperacao).IsAssignableFrom(type))
            {
                entity.HasAlternateKey(
                    nameof(IRegistroOperacao.ServentiaId),
                    nameof(IRegistroOperacao.OperacaoId),
                    nameof(IRegistroOperacao.Id));
                entity.HasIndex(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId));
            }
        }

        modelBuilder.Entity<SegmentoAtoEvidenciaRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.SegmentoId, x.EvidenciaId });
        modelBuilder.Entity<CampoCandidatoEvidenciaRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.CampoId, x.EvidenciaId });
        modelBuilder.Entity<RevisaoAtoCampoRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.RevisaoAtoId, x.RevisaoCampoId });
        modelBuilder.Entity<EventoRevisadoAlvoRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.EventoId, x.PessoaId, x.Papel });
        modelBuilder.Entity<ReconstrucaoEventoRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.ReconstrucaoId, x.EventoId });
        modelBuilder.Entity<SnapshotFonteRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId, x.FonteId });
        modelBuilder.Entity<TitularidadeEvidenciaRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.TitularidadeId, x.EvidenciaId, x.EventoId });
        modelBuilder.Entity<DireitoTitularRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.DireitoId, x.PessoaId });
        modelBuilder.Entity<DireitoEvidenciaRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.DireitoId, x.EvidenciaId });
        modelBuilder.Entity<AchadoEvidenciaRegistro>().HasKey(x => new { x.ServentiaId, x.OperacaoId, x.AchadoId, x.EvidenciaId });
    }

    private static void ConfigurarRelacionamentos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioServentiaRegistro>().HasOne<ServentiaRegistro>().WithMany()
            .HasForeignKey(x => x.ServentiaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UsuarioServentiaRegistro>().HasOne<UsuarioRegistro>().WithMany()
            .HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AutorizacaoRevisaoRegistro>().HasOne<UsuarioServentiaRegistro>().WithMany()
            .HasForeignKey(x => new { x.UsuarioId, x.ServentiaId })
            .HasPrincipalKey(x => new { x.UsuarioId, x.ServentiaId })
            .OnDelete(DeleteBehavior.Restrict);

        foreach (var type in TiposCatalogo.Where(t => typeof(IRegistroTenant).IsAssignableFrom(t)))
        {
            modelBuilder.Entity(type).HasOne(typeof(ServentiaRegistro)).WithMany()
                .HasForeignKey(nameof(IRegistroTenant.ServentiaId)).OnDelete(DeleteBehavior.Restrict);
        }

        foreach (var type in TiposCatalogo.Where(t => typeof(IRegistroOperacao).IsAssignableFrom(t)))
        {
            modelBuilder.Entity(type).HasOne(typeof(OperacaoRegistro)).WithMany()
                .HasForeignKey(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId))
                .HasPrincipalKey(nameof(IRegistroTenant.ServentiaId), nameof(IRegistroTenant.Id))
                .OnDelete(DeleteBehavior.Restrict);
        }

        Operacao(modelBuilder.Entity<PaginaFonteRegistro>(), x => x.FonteId, typeof(FonteDocumentalRegistro));
        Operacao(modelBuilder.Entity<ExecucaoExtracaoRegistro>(), x => x.FonteId, typeof(FonteDocumentalRegistro));
        Operacao(modelBuilder.Entity<PaginaExtraidaRegistro>(), x => x.ExtracaoId, typeof(ExecucaoExtracaoRegistro));
        Operacao(modelBuilder.Entity<PaginaExtraidaRegistro>(), x => x.PaginaFonteId, typeof(PaginaFonteRegistro));
        Operacao(modelBuilder.Entity<EvidenciaFonteRegistro>(), x => x.ExtracaoId, typeof(ExecucaoExtracaoRegistro));
        Operacao(modelBuilder.Entity<EvidenciaFonteRegistro>(), x => x.PaginaExtraidaId, typeof(PaginaExtraidaRegistro));
        Operacao(modelBuilder.Entity<SegmentoAtoRegistro>(), x => x.ExtracaoId, typeof(ExecucaoExtracaoRegistro));
        Operacao(modelBuilder.Entity<ExecucaoInterpretacaoRegistro>(), x => x.SegmentoId, typeof(SegmentoAtoRegistro));
        Operacao(modelBuilder.Entity<AtoCandidatoRegistro>(), x => x.InterpretacaoId, typeof(ExecucaoInterpretacaoRegistro));
        Operacao(modelBuilder.Entity<CampoCandidatoRegistro>(), x => x.AtoCandidatoId, typeof(AtoCandidatoRegistro));
        Operacao(modelBuilder.Entity<RevisaoCampoRegistro>(), x => x.CampoId, typeof(CampoCandidatoRegistro));
        Operacao(modelBuilder.Entity<RevisaoAtoRegistro>(), x => x.AtoId, typeof(AtoCandidatoRegistro));
        Operacao(modelBuilder.Entity<EventoRevisadoRegistro>(), x => x.RevisaoAtoId, typeof(RevisaoAtoRegistro));
        Operacao(modelBuilder.Entity<LacunaReconstrucaoRegistro>(), x => x.ReconstrucaoId, typeof(ExecucaoReconstrucaoRegistro));
        Operacao(modelBuilder.Entity<MatriculaSnapshotRegistro>(), x => x.ReconstrucaoId, typeof(ExecucaoReconstrucaoRegistro));
        Operacao(modelBuilder.Entity<TitularidadeSnapshotRegistro>(), x => x.SnapshotId, typeof(MatriculaSnapshotRegistro));
        Operacao(modelBuilder.Entity<AprovacaoSnapshotRegistro>(), x => x.SnapshotId, typeof(MatriculaSnapshotRegistro));
        Operacao(modelBuilder.Entity<DireitoDocumentadoRegistro>(), x => x.AtoId, typeof(AtoCandidatoRegistro));
        Operacao(modelBuilder.Entity<CenarioRevisaoRegistro>(), x => x.CenarioId, typeof(CenarioRegistro));
        Operacao(modelBuilder.Entity<CenarioRevisaoRegistro>(), x => x.SnapshotId, typeof(MatriculaSnapshotRegistro));
        Operacao(modelBuilder.Entity<TransmissaoPropostaRegistro>(), x => x.CenarioRevisaoId, typeof(CenarioRevisaoRegistro));
        Operacao(modelBuilder.Entity<ExecucaoSimulacaoRegistro>(), x => x.CenarioRevisaoId, typeof(CenarioRevisaoRegistro));
        Operacao(modelBuilder.Entity<ExecucaoSimulacaoRegistro>(), x => x.SnapshotId, typeof(MatriculaSnapshotRegistro));
        Operacao(modelBuilder.Entity<AchadoRegistro>(), x => x.ExecucaoId, typeof(ExecucaoSimulacaoRegistro));
        Operacao(modelBuilder.Entity<RevisaoAchadoRegistro>(), x => x.AchadoId, typeof(AchadoRegistro));
        Operacao(modelBuilder.Entity<TentativaJobRegistro>(), x => x.JobId, typeof(JobProcessamentoRegistro));

        Associacao(modelBuilder.Entity<SegmentoAtoEvidenciaRegistro>(), nameof(SegmentoAtoEvidenciaRegistro.SegmentoId), typeof(SegmentoAtoRegistro));
        Associacao(modelBuilder.Entity<SegmentoAtoEvidenciaRegistro>(), nameof(SegmentoAtoEvidenciaRegistro.EvidenciaId), typeof(EvidenciaFonteRegistro));
        Associacao(modelBuilder.Entity<CampoCandidatoEvidenciaRegistro>(), nameof(CampoCandidatoEvidenciaRegistro.CampoId), typeof(CampoCandidatoRegistro));
        Associacao(modelBuilder.Entity<CampoCandidatoEvidenciaRegistro>(), nameof(CampoCandidatoEvidenciaRegistro.EvidenciaId), typeof(EvidenciaFonteRegistro));
        Associacao(modelBuilder.Entity<RevisaoAtoCampoRegistro>(), nameof(RevisaoAtoCampoRegistro.RevisaoAtoId), typeof(RevisaoAtoRegistro));
        Associacao(modelBuilder.Entity<RevisaoAtoCampoRegistro>(), nameof(RevisaoAtoCampoRegistro.RevisaoCampoId), typeof(RevisaoCampoRegistro));
        Associacao(modelBuilder.Entity<ReconstrucaoEventoRegistro>(), nameof(ReconstrucaoEventoRegistro.ReconstrucaoId), typeof(ExecucaoReconstrucaoRegistro));
        Associacao(modelBuilder.Entity<ReconstrucaoEventoRegistro>(), nameof(ReconstrucaoEventoRegistro.EventoId), typeof(EventoRevisadoRegistro));
        Associacao(modelBuilder.Entity<SnapshotFonteRegistro>(), nameof(SnapshotFonteRegistro.SnapshotId), typeof(MatriculaSnapshotRegistro));
        Associacao(modelBuilder.Entity<SnapshotFonteRegistro>(), nameof(SnapshotFonteRegistro.FonteId), typeof(FonteDocumentalRegistro));
        Associacao(modelBuilder.Entity<DireitoTitularRegistro>(), nameof(DireitoTitularRegistro.DireitoId), typeof(DireitoDocumentadoRegistro));
        Associacao(modelBuilder.Entity<DireitoEvidenciaRegistro>(), nameof(DireitoEvidenciaRegistro.DireitoId), typeof(DireitoDocumentadoRegistro));
        Associacao(modelBuilder.Entity<DireitoEvidenciaRegistro>(), nameof(DireitoEvidenciaRegistro.EvidenciaId), typeof(EvidenciaFonteRegistro));
        Associacao(modelBuilder.Entity<AchadoEvidenciaRegistro>(), nameof(AchadoEvidenciaRegistro.AchadoId), typeof(AchadoRegistro));
        Associacao(modelBuilder.Entity<AchadoEvidenciaRegistro>(), nameof(AchadoEvidenciaRegistro.EvidenciaId), typeof(EvidenciaFonteRegistro));
    }

    private static void ConfigurarRestricoes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioRegistro>()
            .HasIndex(x => new { x.EmissorIdentidade, x.IdentidadeExterna })
            .IsUnique()
            .HasFilter("[EmissorIdentidade] IS NOT NULL AND [IdentidadeExterna] IS NOT NULL");
        modelBuilder.Entity<MatriculaReferenciaRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId }).IsUnique();
        modelBuilder.Entity<FonteDocumentalRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.Sha256, x.Versao }).IsUnique();
        modelBuilder.Entity<PaginaFonteRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.FonteId, x.Numero }).IsUnique();
        modelBuilder.Entity<RevisaoCampoRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.CampoId, x.NumeroVersao }).IsUnique();
        modelBuilder.Entity<RevisaoAtoRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.AtoId, x.NumeroVersao }).IsUnique();
        modelBuilder.Entity<MatriculaSnapshotRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.NumeroVersao }).IsUnique();
        modelBuilder.Entity<CenarioRevisaoRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.CenarioId, x.NumeroVersao }).IsUnique();
        modelBuilder.Entity<TentativaJobRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.JobId, x.Numero }).IsUnique();
        modelBuilder.Entity<JobProcessamentoRegistro>().HasIndex(x => new { x.ServentiaId, x.OperacaoId, x.ChaveIdempotencia }).IsUnique();
        modelBuilder.Entity<JobProcessamentoRegistro>().HasIndex(x => new { x.Estado, x.ProximaExecucaoEmUtc, x.LeaseAteUtc });
        modelBuilder.Entity<EventoAuditoriaRegistro>().HasIndex(x => new { x.ServentiaId, x.EmUtc });

        Check<OperacaoRegistro>(modelBuilder, "CK_Operacao_RevisaoAtual", "[RevisaoAtual] >= 0");
        Check<UsuarioServentiaRegistro>(
            modelBuilder,
            "CK_UsuarioServentia_Perfil",
            "[Perfil] IN ('Operador', 'Conferente', 'ResponsavelJuridico', 'Administrador', 'Auditor')");
        Check<UsuarioRegistro>(
            modelBuilder,
            "CK_Usuario_IdentidadeExterna",
            "([EmissorIdentidade] IS NULL AND [IdentidadeExterna] IS NULL) OR " +
            "([EmissorIdentidade] IS NOT NULL AND [EmissorIdentidade] <> '' AND " +
            "[IdentidadeExterna] IS NOT NULL AND [IdentidadeExterna] <> '')");
        Check<AutorizacaoRevisaoRegistro>(
            modelBuilder,
            "CK_AutorizacaoRevisao_Valores",
            "[VersaoAlvo] > 0 AND DATEPART(TZOFFSET, [EmitidaEmUtc]) = 0 AND " +
            "[UsuarioId] <> '00000000-0000-0000-0000-000000000000' AND " +
            "[RecursoId] <> '00000000-0000-0000-0000-000000000000' AND " +
            "[CorrelationId] <> '00000000-0000-0000-0000-000000000000'");
        Check<FonteDocumentalRegistro>(modelBuilder, "CK_FonteDocumental_Valores", "[Versao] > 0 AND [Bytes] > 0");
        Check<PaginaFonteRegistro>(modelBuilder, "CK_PaginaFonte_NumeroRotacao", "[Numero] > 0 AND [Rotacao] IN (0, 90, 180, 270)");
        Check<ExecucaoExtracaoRegistro>(modelBuilder, "CK_ExecucaoExtracao_Revisao", "[RevisaoConfiguracao] > 0");
        Check<EvidenciaFonteRegistro>(modelBuilder, "CK_EvidenciaFonte_Offsets", "[InicioUtf16] >= 0 AND [FimUtf16] > [InicioUtf16]");
        Check<SegmentoAtoRegistro>(modelBuilder, "CK_SegmentoAto_Ordem", "[OrdemNaFonte] > 0");
        Check<AtoCandidatoRegistro>(modelBuilder, "CK_AtoCandidato_Ordem", "[OrdemDocumental] > 0");
        Check<RevisaoCampoRegistro>(modelBuilder, "CK_RevisaoCampo_VersaoUtc", "[NumeroVersao] > 0 AND DATEPART(TZOFFSET, [EmUtc]) = 0");
        Check<RevisaoAtoRegistro>(modelBuilder, "CK_RevisaoAto_VersaoUtc", "[NumeroVersao] > 0 AND DATEPART(TZOFFSET, [EmUtc]) = 0");
        Check<EventoRevisadoRegistro>(modelBuilder, "CK_EventoRevisado_Fracao", "([FracaoNumerador] IS NULL AND [FracaoDenominador] IS NULL) OR ([FracaoNumerador] >= 0 AND [FracaoDenominador] > 0)");
        Check<TitularidadeSnapshotRegistro>(modelBuilder, "CK_TitularidadeSnapshot_Fracao", "[Numerador] >= 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador]");
        Check<DireitoDocumentadoRegistro>(modelBuilder, "CK_DireitoDocumentado_Fracao", "([Numerador] IS NULL AND [Denominador] IS NULL) OR ([Numerador] > 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador])");
        Check<TransmissaoPropostaRegistro>(modelBuilder, "CK_TransmissaoProposta_Fracao", "[Numerador] > 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador]");
        Check<TentativaJobRegistro>(modelBuilder, "CK_TentativaJob_Numero", "[Numero] > 0");

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(string) && property.Name.EndsWith("Json", StringComparison.Ordinal))
                {
                    var column = property.Name;
                    var nullable = property.IsNullable ? $"[{column}] IS NULL OR " : string.Empty;
                    modelBuilder.Entity(entity.ClrType).ToTable(tb => tb.HasCheckConstraint(
                        $"CK_{entity.GetTableName()}_{column}_Json",
                        $"{nullable}ISJSON([{column}], VALUE) = 1"));
                }

                if (property.ClrType == typeof(string)
                    && (property.Name.EndsWith("Hash", StringComparison.Ordinal) || property.Name == "Sha256"))
                {
                    var column = property.Name;
                    modelBuilder.Entity(entity.ClrType).ToTable(tb => tb.HasCheckConstraint(
                        $"CK_{entity.GetTableName()}_{column}_Sha256",
                        $"LEN([{column}]) = 71 AND LEFT([{column}], 7) = 'sha256:' " +
                        $"AND SUBSTRING([{column}], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'"));
                }
            }


            foreach (var guidColumn in new[] { "Id", "ServentiaId", "OperacaoId" })
            {
                if (entity.FindProperty(guidColumn) is not null)
                {
                    modelBuilder.Entity(entity.ClrType).ToTable(tb => tb.HasCheckConstraint(
                        $"CK_{entity.GetTableName()}_{guidColumn}_NaoVazio",
                        $"[{guidColumn}] <> '00000000-0000-0000-0000-000000000000'"));
                }
            }
        }
    }

    private static void ConfigurarTipos(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
                {
                    property.SetColumnType("datetimeoffset(7)");
                }
                else if (property.ClrType == typeof(DateOnly) || property.ClrType == typeof(DateOnly?))
                {
                    property.SetColumnType("date");
                }
                else if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(5);
                    property.SetScale(4);
                }
                else if (property.ClrType == typeof(string))
                {
                    property.SetMaxLength(Comprimento(property.Name));
                }
            }
        }

        modelBuilder.Entity<OperacaoRegistro>().Property(x => x.RowVersion).IsRowVersion();
        modelBuilder.Entity<CenarioRegistro>().Property(x => x.RowVersion).IsRowVersion();
    }

    private static int? Comprimento(string nome) => nome switch
    {
        var n when n.EndsWith("Json", StringComparison.Ordinal) => null,
        "Trecho" or "Mensagem" or "Justificativa" or "Motivo" => 4000,
        var n when n.EndsWith("StorageKey", StringComparison.Ordinal) => 1024,
        var n when n.Contains("Hash", StringComparison.Ordinal) || n == "Sha256" => 71,
        "Nome" or "NomeExibicao" or "Emissor" or "EmissorIdentidade" or "IdentidadeExterna" or "RotuloLocalizado" => 300,
        "ChaveIdempotencia" => 200,
        _ => 150,
    };

    private static void Operacao<TEntity, TProperty>(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> builder,
        System.Linq.Expressions.Expression<Func<TEntity, TProperty>> foreignKey,
        Type principal)
        where TEntity : class, IRegistroOperacao
    {
        var propertyName = ((System.Linq.Expressions.MemberExpression)foreignKey.Body).Member.Name;
        builder.HasOne(principal).WithMany()
            .HasForeignKey(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId), propertyName)
            .HasPrincipalKey(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId), nameof(IRegistroOperacao.Id))
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void Associacao(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder builder,
        string foreignKey,
        Type principal) => builder.HasOne(principal).WithMany()
        .HasForeignKey(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId), foreignKey)
        .HasPrincipalKey(nameof(IRegistroOperacao.ServentiaId), nameof(IRegistroOperacao.OperacaoId), nameof(IRegistroOperacao.Id))
        .OnDelete(DeleteBehavior.Restrict);

    private static void Check<TEntity>(ModelBuilder modelBuilder, string name, string sql)
        where TEntity : class => modelBuilder.Entity<TEntity>().ToTable(tb => tb.HasCheckConstraint(name, sql));
}
