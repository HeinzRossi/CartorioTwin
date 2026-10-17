using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartorioTwin.Infrastructure.Persistencia.Migrations;

/// <inheritdoc />
public partial class S1T07CatalogoInicial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EventoRevisadoAlvo",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Papel = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventoRevisadoAlvo", x => new { x.ServentiaId, x.OperacaoId, x.EventoId, x.PessoaId, x.Papel });
                table.CheckConstraint("CK_EventoRevisadoAlvo_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EventoRevisadoAlvo_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
            });

        migrationBuilder.CreateTable(
            name: "Serventia",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Uf = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Nome = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Ativa = table.Column<bool>(type: "bit", nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Serventia", x => x.Id);
                table.CheckConstraint("CK_Serventia_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
            });

        migrationBuilder.CreateTable(
            name: "TitularidadeEvidencia",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TitularidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EvidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TitularidadeEvidencia", x => new { x.ServentiaId, x.OperacaoId, x.TitularidadeId, x.EvidenciaId, x.EventoId });
                table.CheckConstraint("CK_TitularidadeEvidencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TitularidadeEvidencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
            });

        migrationBuilder.CreateTable(
            name: "Usuario",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IdentidadeExterna = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                NomeExibicao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                Ativo = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Usuario", x => x.Id);
                table.CheckConstraint("CK_Usuario_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
            });

        migrationBuilder.CreateTable(
            name: "Operacao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoAtual = table.Column<long>(type: "bigint", nullable: false),
                EstadoWorkflow = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                AtualizadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Operacao", x => x.Id);
                table.UniqueConstraint("AK_Operacao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.CheckConstraint("CK_Operacao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_Operacao_RevisaoAtual", "[RevisaoAtual] >= 0");
                table.CheckConstraint("CK_Operacao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_Operacao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PacoteRegra",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Tipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Versao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ConteudoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                Jurisdicao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                VigenciaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                VigenciaFim = table.Column<DateOnly>(type: "date", nullable: true),
                AprovacoesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Situacao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PacoteRegra", x => x.Id);
                table.UniqueConstraint("AK_PacoteRegra_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.CheckConstraint("CK_PacoteRegra_AprovacoesJson_Json", "ISJSON([AprovacoesJson], VALUE) = 1");
                table.CheckConstraint("CK_PacoteRegra_ConteudoHash_Sha256", "LEN([ConteudoHash]) = 71 AND LEFT([ConteudoHash], 7) = 'sha256:' AND SUBSTRING([ConteudoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_PacoteRegra_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PacoteRegra_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_PacoteRegra_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "UsuarioServentia",
            columns: table => new
            {
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Perfil = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Ativo = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsuarioServentia", x => new { x.UsuarioId, x.ServentiaId });
                table.CheckConstraint("CK_UsuarioServentia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_UsuarioServentia_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_UsuarioServentia_Usuario_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuario",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Cenario",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Nome = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                RevisaoAtualId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cenario", x => x.Id);
                table.UniqueConstraint("AK_Cenario_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_Cenario_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_Cenario_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_Cenario_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_Cenario_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_Cenario_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Cenario_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "DossieEmitido",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExecucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ManifestoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                Formato = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                StorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                ConteudoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                EmitidoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DossieEmitido", x => x.Id);
                table.UniqueConstraint("AK_DossieEmitido_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_DossieEmitido_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_DossieEmitido_ConteudoHash_Sha256", "LEN([ConteudoHash]) = 71 AND LEFT([ConteudoHash], 7) = 'sha256:' AND SUBSTRING([ConteudoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_DossieEmitido_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DossieEmitido_ManifestoHash_Sha256", "LEN([ManifestoHash]) = 71 AND LEFT([ManifestoHash], 7) = 'sha256:' AND SUBSTRING([ManifestoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_DossieEmitido_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DossieEmitido_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_DossieEmitido_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DossieEmitido_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "EventoAuditoria",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Acao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Recurso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                RecursoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventoAuditoria", x => x.Id);
                table.UniqueConstraint("AK_EventoAuditoria_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_EventoAuditoria_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_EventoAuditoria_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EventoAuditoria_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EventoAuditoria_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_EventoAuditoria_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EventoAuditoria_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ExecucaoReconstrucao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoCapturada = table.Column<long>(type: "bigint", nullable: false),
                ManifestoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                MotorVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                SaidaStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExecucaoReconstrucao", x => x.Id);
                table.UniqueConstraint("AK_ExecucaoReconstrucao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_ExecucaoReconstrucao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_ExecucaoReconstrucao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoReconstrucao_ManifestoHash_Sha256", "LEN([ManifestoHash]) = 71 AND LEFT([ManifestoHash], 7) = 'sha256:' AND SUBSTRING([ManifestoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_ExecucaoReconstrucao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoReconstrucao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_ExecucaoReconstrucao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoReconstrucao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "FonteDocumental",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Versao = table.Column<int>(type: "int", nullable: false),
                Sha256 = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                StorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                MimeValidado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Bytes = table.Column<long>(type: "bigint", nullable: false),
                DataFonte = table.Column<DateOnly>(type: "date", nullable: true),
                Emissor = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                EstadoTecnico = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FonteDocumental", x => x.Id);
                table.UniqueConstraint("AK_FonteDocumental_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_FonteDocumental_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_FonteDocumental_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_FonteDocumental_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_FonteDocumental_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_FonteDocumental_Sha256_Sha256", "LEN([Sha256]) = 71 AND LEFT([Sha256], 7) = 'sha256:' AND SUBSTRING([Sha256], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_FonteDocumental_Valores", "[Versao] > 0 AND [Bytes] > 0");
                table.ForeignKey(
                    name: "FK_FonteDocumental_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_FonteDocumental_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "JobProcessamento",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FonteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Tipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ChaveIdempotencia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ManifestoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                Estado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ProximaExecucaoEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                LeaseAteUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                WorkerId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                FencingToken = table.Column<long>(type: "bigint", nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JobProcessamento", x => x.Id);
                table.UniqueConstraint("AK_JobProcessamento_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_JobProcessamento_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_JobProcessamento_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_JobProcessamento_ManifestoHash_Sha256", "LEN([ManifestoHash]) = 71 AND LEFT([ManifestoHash], 7) = 'sha256:' AND SUBSTRING([ManifestoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_JobProcessamento_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_JobProcessamento_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_JobProcessamento_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_JobProcessamento_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "MatriculaReferencia",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaOrigem = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Numero = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Cnm = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                IdentificacaoRevisada = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatriculaReferencia", x => x.Id);
                table.UniqueConstraint("AK_MatriculaReferencia_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_MatriculaReferencia_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_MatriculaReferencia_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_MatriculaReferencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_MatriculaReferencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_MatriculaReferencia_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_MatriculaReferencia_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ValidadeExecucao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExecucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Situacao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Motivo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                RevisaoSubstitutaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                AtualizadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ValidadeExecucao", x => x.Id);
                table.UniqueConstraint("AK_ValidadeExecucao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_ValidadeExecucao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_ValidadeExecucao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ValidadeExecucao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ValidadeExecucao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_ValidadeExecucao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ValidadeExecucao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "VinculoAtosFontes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtoOrigemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtoRelacionadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TipoRelacao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                RevisaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VinculoAtosFontes", x => x.Id);
                table.UniqueConstraint("AK_VinculoAtosFontes_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_VinculoAtosFontes_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_VinculoAtosFontes_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_VinculoAtosFontes_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_VinculoAtosFontes_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_VinculoAtosFontes_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_VinculoAtosFontes_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "LacunaReconstrucao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReconstrucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Tipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Relevancia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ReferenciasJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Motivo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LacunaReconstrucao", x => x.Id);
                table.UniqueConstraint("AK_LacunaReconstrucao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_LacunaReconstrucao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_LacunaReconstrucao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_LacunaReconstrucao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_LacunaReconstrucao_ReferenciasJson_Json", "ISJSON([ReferenciasJson], VALUE) = 1");
                table.CheckConstraint("CK_LacunaReconstrucao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_LacunaReconstrucao_ExecucaoReconstrucao_ServentiaId_OperacaoId_ReconstrucaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ReconstrucaoId },
                    principalTable: "ExecucaoReconstrucao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_LacunaReconstrucao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_LacunaReconstrucao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "MatriculaSnapshot",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReconstrucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NumeroVersao = table.Column<int>(type: "int", nullable: false),
                Hash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                Completude = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ManifestoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatriculaSnapshot", x => x.Id);
                table.UniqueConstraint("AK_MatriculaSnapshot_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_MatriculaSnapshot_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_MatriculaSnapshot_Hash_Sha256", "LEN([Hash]) = 71 AND LEFT([Hash], 7) = 'sha256:' AND SUBSTRING([Hash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_MatriculaSnapshot_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_MatriculaSnapshot_ManifestoHash_Sha256", "LEN([ManifestoHash]) = 71 AND LEFT([ManifestoHash], 7) = 'sha256:' AND SUBSTRING([ManifestoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_MatriculaSnapshot_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_MatriculaSnapshot_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_MatriculaSnapshot_ExecucaoReconstrucao_ServentiaId_OperacaoId_ReconstrucaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ReconstrucaoId },
                    principalTable: "ExecucaoReconstrucao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_MatriculaSnapshot_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_MatriculaSnapshot_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ExecucaoExtracao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FonteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoConfiguracao = table.Column<int>(type: "int", nullable: false),
                VersoesMotoresJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Estado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ManifestoJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CriadaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExecucaoExtracao", x => x.Id);
                table.UniqueConstraint("AK_ExecucaoExtracao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_ExecucaoExtracao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_ExecucaoExtracao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoExtracao_ManifestoJson_Json", "ISJSON([ManifestoJson], VALUE) = 1");
                table.CheckConstraint("CK_ExecucaoExtracao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoExtracao_Revisao", "[RevisaoConfiguracao] > 0");
                table.CheckConstraint("CK_ExecucaoExtracao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoExtracao_VersoesMotoresJson_Json", "ISJSON([VersoesMotoresJson], VALUE) = 1");
                table.ForeignKey(
                    name: "FK_ExecucaoExtracao_FonteDocumental_ServentiaId_OperacaoId_FonteId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.FonteId },
                    principalTable: "FonteDocumental",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoExtracao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoExtracao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PaginaFonte",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FonteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Numero = table.Column<int>(type: "int", nullable: false),
                DimensoesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Rotacao = table.Column<short>(type: "smallint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaginaFonte", x => x.Id);
                table.UniqueConstraint("AK_PaginaFonte_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_PaginaFonte_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_PaginaFonte_DimensoesJson_Json", "[DimensoesJson] IS NULL OR ISJSON([DimensoesJson], VALUE) = 1");
                table.CheckConstraint("CK_PaginaFonte_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PaginaFonte_NumeroRotacao", "[Numero] > 0 AND [Rotacao] IN (0, 90, 180, 270)");
                table.CheckConstraint("CK_PaginaFonte_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PaginaFonte_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_PaginaFonte_FonteDocumental_ServentiaId_OperacaoId_FonteId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.FonteId },
                    principalTable: "FonteDocumental",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PaginaFonte_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PaginaFonte_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TentativaJob",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Numero = table.Column<int>(type: "int", nullable: false),
                InicioUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                FimUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                Estado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ErroCodigo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                ArtefatosJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ConfiguracaoFixadaJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TentativaJob", x => x.Id);
                table.UniqueConstraint("AK_TentativaJob_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_TentativaJob_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_TentativaJob_ArtefatosJson_Json", "ISJSON([ArtefatosJson], VALUE) = 1");
                table.CheckConstraint("CK_TentativaJob_ConfiguracaoFixadaJson_Json", "ISJSON([ConfiguracaoFixadaJson], VALUE) = 1");
                table.CheckConstraint("CK_TentativaJob_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TentativaJob_Numero", "[Numero] > 0");
                table.CheckConstraint("CK_TentativaJob_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TentativaJob_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_TentativaJob_JobProcessamento_ServentiaId_OperacaoId_JobId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.JobId },
                    principalTable: "JobProcessamento",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TentativaJob_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TentativaJob_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AprovacaoSnapshot",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SnapshotHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                RevisaoOperacao = table.Column<long>(type: "bigint", nullable: false),
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                PremissasJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Justificativa = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AprovacaoSnapshot", x => x.Id);
                table.UniqueConstraint("AK_AprovacaoSnapshot_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_AprovacaoSnapshot_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_AprovacaoSnapshot_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AprovacaoSnapshot_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AprovacaoSnapshot_PremissasJson_Json", "ISJSON([PremissasJson], VALUE) = 1");
                table.CheckConstraint("CK_AprovacaoSnapshot_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AprovacaoSnapshot_SnapshotHash_Sha256", "LEN([SnapshotHash]) = 71 AND LEFT([SnapshotHash], 7) = 'sha256:' AND SUBSTRING([SnapshotHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.ForeignKey(
                    name: "FK_AprovacaoSnapshot_MatriculaSnapshot_ServentiaId_OperacaoId_SnapshotId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId },
                    principalTable: "MatriculaSnapshot",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AprovacaoSnapshot_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AprovacaoSnapshot_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CenarioRevisao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NumeroVersao = table.Column<int>(type: "int", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PropostaHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CenarioRevisao", x => x.Id);
                table.UniqueConstraint("AK_CenarioRevisao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_CenarioRevisao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_CenarioRevisao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CenarioRevisao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CenarioRevisao_PropostaHash_Sha256", "LEN([PropostaHash]) = 71 AND LEFT([PropostaHash], 7) = 'sha256:' AND SUBSTRING([PropostaHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_CenarioRevisao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_CenarioRevisao_Cenario_ServentiaId_OperacaoId_CenarioId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.CenarioId },
                    principalTable: "Cenario",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CenarioRevisao_MatriculaSnapshot_ServentiaId_OperacaoId_SnapshotId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId },
                    principalTable: "MatriculaSnapshot",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CenarioRevisao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CenarioRevisao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SnapshotFonte",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FonteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Finalidade = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SnapshotFonte", x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId, x.FonteId });
                table.CheckConstraint("CK_SnapshotFonte_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_SnapshotFonte_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_SnapshotFonte_FonteDocumental_ServentiaId_OperacaoId_FonteId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.FonteId },
                    principalTable: "FonteDocumental",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SnapshotFonte_MatriculaSnapshot_ServentiaId_OperacaoId_SnapshotId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId },
                    principalTable: "MatriculaSnapshot",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TitularidadeSnapshot",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TipoDireito = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Numerador = table.Column<long>(type: "bigint", nullable: false),
                Denominador = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TitularidadeSnapshot", x => x.Id);
                table.UniqueConstraint("AK_TitularidadeSnapshot_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_TitularidadeSnapshot_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_TitularidadeSnapshot_Fracao", "[Numerador] >= 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador]");
                table.CheckConstraint("CK_TitularidadeSnapshot_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TitularidadeSnapshot_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TitularidadeSnapshot_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_TitularidadeSnapshot_MatriculaSnapshot_ServentiaId_OperacaoId_SnapshotId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId },
                    principalTable: "MatriculaSnapshot",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TitularidadeSnapshot_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TitularidadeSnapshot_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SegmentoAto",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExtracaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OrdemNaFonte = table.Column<int>(type: "int", nullable: false),
                RotuloLocalizado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SegmentoAto", x => x.Id);
                table.UniqueConstraint("AK_SegmentoAto_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_SegmentoAto_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_SegmentoAto_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_SegmentoAto_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_SegmentoAto_Ordem", "[OrdemNaFonte] > 0");
                table.CheckConstraint("CK_SegmentoAto_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_SegmentoAto_ExecucaoExtracao_ServentiaId_OperacaoId_ExtracaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ExtracaoId },
                    principalTable: "ExecucaoExtracao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SegmentoAto_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SegmentoAto_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PaginaExtraida",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExtracaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaginaFonteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Estrategia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                TextoStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                LayoutStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                TextoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                Qualidade = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaginaExtraida", x => x.Id);
                table.UniqueConstraint("AK_PaginaExtraida_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_PaginaExtraida_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_PaginaExtraida_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PaginaExtraida_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PaginaExtraida_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_PaginaExtraida_TextoHash_Sha256", "LEN([TextoHash]) = 71 AND LEFT([TextoHash], 7) = 'sha256:' AND SUBSTRING([TextoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.ForeignKey(
                    name: "FK_PaginaExtraida_ExecucaoExtracao_ServentiaId_OperacaoId_ExtracaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ExtracaoId },
                    principalTable: "ExecucaoExtracao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PaginaExtraida_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PaginaExtraida_PaginaFonte_ServentiaId_OperacaoId_PaginaFonteId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.PaginaFonteId },
                    principalTable: "PaginaFonte",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_PaginaExtraida_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ExecucaoSimulacao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CenarioRevisaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ManifestoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                EntradasStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                ResultadoStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                MotorVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Conclusao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Completude = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Cobertura = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExecucaoSimulacao", x => x.Id);
                table.UniqueConstraint("AK_ExecucaoSimulacao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_ExecucaoSimulacao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_ExecucaoSimulacao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoSimulacao_ManifestoHash_Sha256", "LEN([ManifestoHash]) = 71 AND LEFT([ManifestoHash], 7) = 'sha256:' AND SUBSTRING([ManifestoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_ExecucaoSimulacao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoSimulacao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_ExecucaoSimulacao_CenarioRevisao_ServentiaId_OperacaoId_CenarioRevisaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.CenarioRevisaoId },
                    principalTable: "CenarioRevisao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoSimulacao_MatriculaSnapshot_ServentiaId_OperacaoId_SnapshotId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SnapshotId },
                    principalTable: "MatriculaSnapshot",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoSimulacao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoSimulacao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TransmissaoProposta",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CenarioRevisaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AlienanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AdquirenteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TipoDireito = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                BaseFracao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Numerador = table.Column<long>(type: "bigint", nullable: false),
                Denominador = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TransmissaoProposta", x => x.Id);
                table.UniqueConstraint("AK_TransmissaoProposta_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_TransmissaoProposta_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_TransmissaoProposta_Fracao", "[Numerador] > 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador]");
                table.CheckConstraint("CK_TransmissaoProposta_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TransmissaoProposta_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_TransmissaoProposta_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_TransmissaoProposta_CenarioRevisao_ServentiaId_OperacaoId_CenarioRevisaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.CenarioRevisaoId },
                    principalTable: "CenarioRevisao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TransmissaoProposta_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TransmissaoProposta_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ExecucaoInterpretacao",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SegmentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProvedorVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                PromptHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                SchemaVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ConfiguracaoHash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false),
                SaidaStorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                IssuesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExecucaoInterpretacao", x => x.Id);
                table.UniqueConstraint("AK_ExecucaoInterpretacao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_ExecucaoInterpretacao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_ExecucaoInterpretacao_ConfiguracaoHash_Sha256", "LEN([ConfiguracaoHash]) = 71 AND LEFT([ConfiguracaoHash], 7) = 'sha256:' AND SUBSTRING([ConfiguracaoHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_ExecucaoInterpretacao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoInterpretacao_IssuesJson_Json", "ISJSON([IssuesJson], VALUE) = 1");
                table.CheckConstraint("CK_ExecucaoInterpretacao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ExecucaoInterpretacao_PromptHash_Sha256", "LEN([PromptHash]) = 71 AND LEFT([PromptHash], 7) = 'sha256:' AND SUBSTRING([PromptHash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_ExecucaoInterpretacao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_ExecucaoInterpretacao_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoInterpretacao_SegmentoAto_ServentiaId_OperacaoId_SegmentoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SegmentoId },
                    principalTable: "SegmentoAto",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ExecucaoInterpretacao_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "EvidenciaFonte",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExtracaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaginaExtraidaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InicioUtf16 = table.Column<int>(type: "int", nullable: false),
                FimUtf16 = table.Column<int>(type: "int", nullable: false),
                Trecho = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                BoundingBoxJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RevisaoVisualJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EvidenciaFonte", x => x.Id);
                table.UniqueConstraint("AK_EvidenciaFonte_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_EvidenciaFonte_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_EvidenciaFonte_BoundingBoxJson_Json", "[BoundingBoxJson] IS NULL OR ISJSON([BoundingBoxJson], VALUE) = 1");
                table.CheckConstraint("CK_EvidenciaFonte_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EvidenciaFonte_Offsets", "[InicioUtf16] >= 0 AND [FimUtf16] > [InicioUtf16]");
                table.CheckConstraint("CK_EvidenciaFonte_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EvidenciaFonte_RevisaoVisualJson_Json", "[RevisaoVisualJson] IS NULL OR ISJSON([RevisaoVisualJson], VALUE) = 1");
                table.CheckConstraint("CK_EvidenciaFonte_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_EvidenciaFonte_ExecucaoExtracao_ServentiaId_OperacaoId_ExtracaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ExtracaoId },
                    principalTable: "ExecucaoExtracao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EvidenciaFonte_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EvidenciaFonte_PaginaExtraida_ServentiaId_OperacaoId_PaginaExtraidaId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.PaginaExtraidaId },
                    principalTable: "PaginaExtraida",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EvidenciaFonte_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Achado",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ExecucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RegraCodigo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                RegraVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Severidade = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                FatosComparadosJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Mensagem = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Achado", x => x.Id);
                table.UniqueConstraint("AK_Achado_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_Achado_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_Achado_FatosComparadosJson_Json", "ISJSON([FatosComparadosJson], VALUE) = 1");
                table.CheckConstraint("CK_Achado_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_Achado_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_Achado_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_Achado_ExecucaoSimulacao_ServentiaId_OperacaoId_ExecucaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ExecucaoId },
                    principalTable: "ExecucaoSimulacao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Achado_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Achado_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AtoCandidato",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InterpretacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                IdentificadorSugerido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                TipoSugerido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                OrdemDocumental = table.Column<int>(type: "int", nullable: false),
                DatasSegregadasJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IssuesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AtoCandidato", x => x.Id);
                table.UniqueConstraint("AK_AtoCandidato_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_AtoCandidato_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_AtoCandidato_DatasSegregadasJson_Json", "ISJSON([DatasSegregadasJson], VALUE) = 1");
                table.CheckConstraint("CK_AtoCandidato_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AtoCandidato_IssuesJson_Json", "ISJSON([IssuesJson], VALUE) = 1");
                table.CheckConstraint("CK_AtoCandidato_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AtoCandidato_Ordem", "[OrdemDocumental] > 0");
                table.CheckConstraint("CK_AtoCandidato_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_AtoCandidato_ExecucaoInterpretacao_ServentiaId_OperacaoId_InterpretacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.InterpretacaoId },
                    principalTable: "ExecucaoInterpretacao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AtoCandidato_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AtoCandidato_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SegmentoAtoEvidencia",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SegmentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EvidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SegmentoAtoEvidencia", x => new { x.ServentiaId, x.OperacaoId, x.SegmentoId, x.EvidenciaId });
                table.CheckConstraint("CK_SegmentoAtoEvidencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_SegmentoAtoEvidencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_SegmentoAtoEvidencia_EvidenciaFonte_ServentiaId_OperacaoId_EvidenciaId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.EvidenciaId },
                    principalTable: "EvidenciaFonte",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_SegmentoAtoEvidencia_SegmentoAto_ServentiaId_OperacaoId_SegmentoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.SegmentoId },
                    principalTable: "SegmentoAto",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AchadoEvidencia",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AchadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EvidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AchadoEvidencia", x => new { x.ServentiaId, x.OperacaoId, x.AchadoId, x.EvidenciaId });
                table.CheckConstraint("CK_AchadoEvidencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_AchadoEvidencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_AchadoEvidencia_Achado_ServentiaId_OperacaoId_AchadoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.AchadoId },
                    principalTable: "Achado",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AchadoEvidencia_EvidenciaFonte_ServentiaId_OperacaoId_EvidenciaId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.EvidenciaId },
                    principalTable: "EvidenciaFonte",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RevisaoAchado",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AchadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Decisao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Justificativa = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                EmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RevisaoAchado", x => x.Id);
                table.UniqueConstraint("AK_RevisaoAchado_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_RevisaoAchado_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_RevisaoAchado_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAchado_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAchado_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_RevisaoAchado_Achado_ServentiaId_OperacaoId_AchadoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.AchadoId },
                    principalTable: "Achado",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoAchado_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoAchado_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CampoCandidato",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtoCandidatoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CaminhoCampo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ValorSugeridoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Tipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Estado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CampoCandidato", x => x.Id);
                table.UniqueConstraint("AK_CampoCandidato_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_CampoCandidato_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_CampoCandidato_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CampoCandidato_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CampoCandidato_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CampoCandidato_ValorSugeridoJson_Json", "[ValorSugeridoJson] IS NULL OR ISJSON([ValorSugeridoJson], VALUE) = 1");
                table.ForeignKey(
                    name: "FK_CampoCandidato_AtoCandidato_ServentiaId_OperacaoId_AtoCandidatoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.AtoCandidatoId },
                    principalTable: "AtoCandidato",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CampoCandidato_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CampoCandidato_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "DireitoDocumentado",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MatriculaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Tipo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                TipoExtensao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Numerador = table.Column<long>(type: "bigint", nullable: true),
                Denominador = table.Column<long>(type: "bigint", nullable: true),
                ConhecimentoEfeito = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                InicioDocumentado = table.Column<DateOnly>(type: "date", nullable: true),
                FimDocumentado = table.Column<DateOnly>(type: "date", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DireitoDocumentado", x => x.Id);
                table.UniqueConstraint("AK_DireitoDocumentado_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_DireitoDocumentado_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_DireitoDocumentado_Fracao", "([Numerador] IS NULL AND [Denominador] IS NULL) OR ([Numerador] > 0 AND [Denominador] > 0 AND [Numerador] <= [Denominador])");
                table.CheckConstraint("CK_DireitoDocumentado_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DireitoDocumentado_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DireitoDocumentado_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_DireitoDocumentado_AtoCandidato_ServentiaId_OperacaoId_AtoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.AtoId },
                    principalTable: "AtoCandidato",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DireitoDocumentado_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DireitoDocumentado_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RevisaoAto",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AtoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NumeroVersao = table.Column<int>(type: "int", nullable: false),
                Decisao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                Hash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RevisaoAto", x => x.Id);
                table.UniqueConstraint("AK_RevisaoAto_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_RevisaoAto_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_RevisaoAto_Hash_Sha256", "LEN([Hash]) = 71 AND LEFT([Hash], 7) = 'sha256:' AND SUBSTRING([Hash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_RevisaoAto_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAto_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAto_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAto_VersaoUtc", "[NumeroVersao] > 0 AND DATEPART(TZOFFSET, [EmUtc]) = 0");
                table.ForeignKey(
                    name: "FK_RevisaoAto_AtoCandidato_ServentiaId_OperacaoId_AtoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.AtoId },
                    principalTable: "AtoCandidato",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoAto_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoAto_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CampoCandidatoEvidencia",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CampoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EvidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CampoCandidatoEvidencia", x => new { x.ServentiaId, x.OperacaoId, x.CampoId, x.EvidenciaId });
                table.CheckConstraint("CK_CampoCandidatoEvidencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_CampoCandidatoEvidencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_CampoCandidatoEvidencia_CampoCandidato_ServentiaId_OperacaoId_CampoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.CampoId },
                    principalTable: "CampoCandidato",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_CampoCandidatoEvidencia_EvidenciaFonte_ServentiaId_OperacaoId_EvidenciaId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.EvidenciaId },
                    principalTable: "EvidenciaFonte",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RevisaoCampo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CampoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                NumeroVersao = table.Column<int>(type: "int", nullable: false),
                ValorFinalJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Decisao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Justificativa = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AutorizacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RevisaoCampo", x => x.Id);
                table.UniqueConstraint("AK_RevisaoCampo_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_RevisaoCampo_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_RevisaoCampo_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoCampo_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoCampo_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoCampo_ValorFinalJson_Json", "[ValorFinalJson] IS NULL OR ISJSON([ValorFinalJson], VALUE) = 1");
                table.CheckConstraint("CK_RevisaoCampo_VersaoUtc", "[NumeroVersao] > 0 AND DATEPART(TZOFFSET, [EmUtc]) = 0");
                table.ForeignKey(
                    name: "FK_RevisaoCampo_CampoCandidato_ServentiaId_OperacaoId_CampoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.CampoId },
                    principalTable: "CampoCandidato",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoCampo_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoCampo_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "DireitoEvidencia",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DireitoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EvidenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DireitoEvidencia", x => new { x.ServentiaId, x.OperacaoId, x.DireitoId, x.EvidenciaId });
                table.CheckConstraint("CK_DireitoEvidencia_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DireitoEvidencia_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_DireitoEvidencia_DireitoDocumentado_ServentiaId_OperacaoId_DireitoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.DireitoId },
                    principalTable: "DireitoDocumentado",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DireitoEvidencia_EvidenciaFonte_ServentiaId_OperacaoId_EvidenciaId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.EvidenciaId },
                    principalTable: "EvidenciaFonte",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "DireitoTitular",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DireitoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PessoaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DireitoTitular", x => new { x.ServentiaId, x.OperacaoId, x.DireitoId, x.PessoaId });
                table.CheckConstraint("CK_DireitoTitular_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_DireitoTitular_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_DireitoTitular_DireitoDocumentado_ServentiaId_OperacaoId_DireitoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.DireitoId },
                    principalTable: "DireitoDocumentado",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "EventoRevisado",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoAtoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TipoEvento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                FracaoNumerador = table.Column<long>(type: "bigint", nullable: true),
                FracaoDenominador = table.Column<long>(type: "bigint", nullable: true),
                OrdemRevisada = table.Column<int>(type: "int", nullable: false),
                MapeamentoVersao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                PremissasJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Hash = table.Column<string>(type: "nvarchar(71)", maxLength: 71, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventoRevisado", x => x.Id);
                table.UniqueConstraint("AK_EventoRevisado_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                table.UniqueConstraint("AK_EventoRevisado_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                table.CheckConstraint("CK_EventoRevisado_Fracao", "([FracaoNumerador] IS NULL AND [FracaoDenominador] IS NULL) OR ([FracaoNumerador] >= 0 AND [FracaoDenominador] > 0)");
                table.CheckConstraint("CK_EventoRevisado_Hash_Sha256", "LEN([Hash]) = 71 AND LEFT([Hash], 7) = 'sha256:' AND SUBSTRING([Hash], 8, 64) COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9a-f]%'");
                table.CheckConstraint("CK_EventoRevisado_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EventoRevisado_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_EventoRevisado_PremissasJson_Json", "ISJSON([PremissasJson], VALUE) = 1");
                table.CheckConstraint("CK_EventoRevisado_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_EventoRevisado_Operacao_ServentiaId_OperacaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId },
                    principalTable: "Operacao",
                    principalColumns: new[] { "ServentiaId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EventoRevisado_RevisaoAto_ServentiaId_OperacaoId_RevisaoAtoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.RevisaoAtoId },
                    principalTable: "RevisaoAto",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_EventoRevisado_Serventia_ServentiaId",
                    column: x => x.ServentiaId,
                    principalTable: "Serventia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "RevisaoAtoCampo",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoAtoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RevisaoCampoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RevisaoAtoCampo", x => new { x.ServentiaId, x.OperacaoId, x.RevisaoAtoId, x.RevisaoCampoId });
                table.CheckConstraint("CK_RevisaoAtoCampo_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_RevisaoAtoCampo_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_RevisaoAtoCampo_RevisaoAto_ServentiaId_OperacaoId_RevisaoAtoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.RevisaoAtoId },
                    principalTable: "RevisaoAto",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_RevisaoAtoCampo_RevisaoCampo_ServentiaId_OperacaoId_RevisaoCampoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.RevisaoCampoId },
                    principalTable: "RevisaoCampo",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ReconstrucaoEvento",
            columns: table => new
            {
                ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReconstrucaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReconstrucaoEvento", x => new { x.ServentiaId, x.OperacaoId, x.ReconstrucaoId, x.EventoId });
                table.CheckConstraint("CK_ReconstrucaoEvento_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                table.CheckConstraint("CK_ReconstrucaoEvento_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                table.ForeignKey(
                    name: "FK_ReconstrucaoEvento_EventoRevisado_ServentiaId_OperacaoId_EventoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.EventoId },
                    principalTable: "EventoRevisado",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ReconstrucaoEvento_ExecucaoReconstrucao_ServentiaId_OperacaoId_ReconstrucaoId",
                    columns: x => new { x.ServentiaId, x.OperacaoId, x.ReconstrucaoId },
                    principalTable: "ExecucaoReconstrucao",
                    principalColumns: new[] { "ServentiaId", "OperacaoId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Achado_ServentiaId",
            table: "Achado",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_Achado_ServentiaId_OperacaoId",
            table: "Achado",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_Achado_ServentiaId_OperacaoId_ExecucaoId",
            table: "Achado",
            columns: new[] { "ServentiaId", "OperacaoId", "ExecucaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_AchadoEvidencia_ServentiaId_OperacaoId_EvidenciaId",
            table: "AchadoEvidencia",
            columns: new[] { "ServentiaId", "OperacaoId", "EvidenciaId" });

        migrationBuilder.CreateIndex(
            name: "IX_AprovacaoSnapshot_ServentiaId",
            table: "AprovacaoSnapshot",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_AprovacaoSnapshot_ServentiaId_OperacaoId",
            table: "AprovacaoSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_AprovacaoSnapshot_ServentiaId_OperacaoId_SnapshotId",
            table: "AprovacaoSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId", "SnapshotId" });

        migrationBuilder.CreateIndex(
            name: "IX_AtoCandidato_ServentiaId",
            table: "AtoCandidato",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_AtoCandidato_ServentiaId_OperacaoId",
            table: "AtoCandidato",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_AtoCandidato_ServentiaId_OperacaoId_InterpretacaoId",
            table: "AtoCandidato",
            columns: new[] { "ServentiaId", "OperacaoId", "InterpretacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_CampoCandidato_ServentiaId",
            table: "CampoCandidato",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_CampoCandidato_ServentiaId_OperacaoId",
            table: "CampoCandidato",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_CampoCandidato_ServentiaId_OperacaoId_AtoCandidatoId",
            table: "CampoCandidato",
            columns: new[] { "ServentiaId", "OperacaoId", "AtoCandidatoId" });

        migrationBuilder.CreateIndex(
            name: "IX_CampoCandidatoEvidencia_ServentiaId_OperacaoId_EvidenciaId",
            table: "CampoCandidatoEvidencia",
            columns: new[] { "ServentiaId", "OperacaoId", "EvidenciaId" });

        migrationBuilder.CreateIndex(
            name: "IX_Cenario_ServentiaId",
            table: "Cenario",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_Cenario_ServentiaId_OperacaoId",
            table: "Cenario",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_CenarioRevisao_ServentiaId",
            table: "CenarioRevisao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_CenarioRevisao_ServentiaId_OperacaoId",
            table: "CenarioRevisao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_CenarioRevisao_ServentiaId_OperacaoId_CenarioId_NumeroVersao",
            table: "CenarioRevisao",
            columns: new[] { "ServentiaId", "OperacaoId", "CenarioId", "NumeroVersao" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CenarioRevisao_ServentiaId_OperacaoId_SnapshotId",
            table: "CenarioRevisao",
            columns: new[] { "ServentiaId", "OperacaoId", "SnapshotId" });

        migrationBuilder.CreateIndex(
            name: "IX_DireitoDocumentado_ServentiaId",
            table: "DireitoDocumentado",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_DireitoDocumentado_ServentiaId_OperacaoId",
            table: "DireitoDocumentado",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_DireitoDocumentado_ServentiaId_OperacaoId_AtoId",
            table: "DireitoDocumentado",
            columns: new[] { "ServentiaId", "OperacaoId", "AtoId" });

        migrationBuilder.CreateIndex(
            name: "IX_DireitoEvidencia_ServentiaId_OperacaoId_EvidenciaId",
            table: "DireitoEvidencia",
            columns: new[] { "ServentiaId", "OperacaoId", "EvidenciaId" });

        migrationBuilder.CreateIndex(
            name: "IX_DossieEmitido_ServentiaId",
            table: "DossieEmitido",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_DossieEmitido_ServentiaId_OperacaoId",
            table: "DossieEmitido",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EventoAuditoria_ServentiaId",
            table: "EventoAuditoria",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_EventoAuditoria_ServentiaId_EmUtc",
            table: "EventoAuditoria",
            columns: new[] { "ServentiaId", "EmUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_EventoAuditoria_ServentiaId_OperacaoId",
            table: "EventoAuditoria",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EventoRevisado_ServentiaId",
            table: "EventoRevisado",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_EventoRevisado_ServentiaId_OperacaoId",
            table: "EventoRevisado",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EventoRevisado_ServentiaId_OperacaoId_RevisaoAtoId",
            table: "EventoRevisado",
            columns: new[] { "ServentiaId", "OperacaoId", "RevisaoAtoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EvidenciaFonte_ServentiaId",
            table: "EvidenciaFonte",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_EvidenciaFonte_ServentiaId_OperacaoId",
            table: "EvidenciaFonte",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EvidenciaFonte_ServentiaId_OperacaoId_ExtracaoId",
            table: "EvidenciaFonte",
            columns: new[] { "ServentiaId", "OperacaoId", "ExtracaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_EvidenciaFonte_ServentiaId_OperacaoId_PaginaExtraidaId",
            table: "EvidenciaFonte",
            columns: new[] { "ServentiaId", "OperacaoId", "PaginaExtraidaId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoExtracao_ServentiaId",
            table: "ExecucaoExtracao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoExtracao_ServentiaId_OperacaoId",
            table: "ExecucaoExtracao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoExtracao_ServentiaId_OperacaoId_FonteId",
            table: "ExecucaoExtracao",
            columns: new[] { "ServentiaId", "OperacaoId", "FonteId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoInterpretacao_ServentiaId",
            table: "ExecucaoInterpretacao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoInterpretacao_ServentiaId_OperacaoId",
            table: "ExecucaoInterpretacao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoInterpretacao_ServentiaId_OperacaoId_SegmentoId",
            table: "ExecucaoInterpretacao",
            columns: new[] { "ServentiaId", "OperacaoId", "SegmentoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoReconstrucao_ServentiaId",
            table: "ExecucaoReconstrucao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoReconstrucao_ServentiaId_OperacaoId",
            table: "ExecucaoReconstrucao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoSimulacao_ServentiaId",
            table: "ExecucaoSimulacao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoSimulacao_ServentiaId_OperacaoId",
            table: "ExecucaoSimulacao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoSimulacao_ServentiaId_OperacaoId_CenarioRevisaoId",
            table: "ExecucaoSimulacao",
            columns: new[] { "ServentiaId", "OperacaoId", "CenarioRevisaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_ExecucaoSimulacao_ServentiaId_OperacaoId_SnapshotId",
            table: "ExecucaoSimulacao",
            columns: new[] { "ServentiaId", "OperacaoId", "SnapshotId" });

        migrationBuilder.CreateIndex(
            name: "IX_FonteDocumental_ServentiaId",
            table: "FonteDocumental",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_FonteDocumental_ServentiaId_OperacaoId",
            table: "FonteDocumental",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_FonteDocumental_ServentiaId_OperacaoId_Sha256_Versao",
            table: "FonteDocumental",
            columns: new[] { "ServentiaId", "OperacaoId", "Sha256", "Versao" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobProcessamento_Estado_ProximaExecucaoEmUtc_LeaseAteUtc",
            table: "JobProcessamento",
            columns: new[] { "Estado", "ProximaExecucaoEmUtc", "LeaseAteUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_JobProcessamento_ServentiaId",
            table: "JobProcessamento",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_JobProcessamento_ServentiaId_OperacaoId",
            table: "JobProcessamento",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_JobProcessamento_ServentiaId_OperacaoId_ChaveIdempotencia",
            table: "JobProcessamento",
            columns: new[] { "ServentiaId", "OperacaoId", "ChaveIdempotencia" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LacunaReconstrucao_ServentiaId",
            table: "LacunaReconstrucao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_LacunaReconstrucao_ServentiaId_OperacaoId",
            table: "LacunaReconstrucao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_LacunaReconstrucao_ServentiaId_OperacaoId_ReconstrucaoId",
            table: "LacunaReconstrucao",
            columns: new[] { "ServentiaId", "OperacaoId", "ReconstrucaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaReferencia_ServentiaId",
            table: "MatriculaReferencia",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaReferencia_ServentiaId_OperacaoId",
            table: "MatriculaReferencia",
            columns: new[] { "ServentiaId", "OperacaoId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaSnapshot_ServentiaId",
            table: "MatriculaSnapshot",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaSnapshot_ServentiaId_OperacaoId",
            table: "MatriculaSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaSnapshot_ServentiaId_OperacaoId_NumeroVersao",
            table: "MatriculaSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId", "NumeroVersao" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_MatriculaSnapshot_ServentiaId_OperacaoId_ReconstrucaoId",
            table: "MatriculaSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId", "ReconstrucaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_Operacao_ServentiaId",
            table: "Operacao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_PacoteRegra_ServentiaId",
            table: "PacoteRegra",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_PaginaExtraida_ServentiaId",
            table: "PaginaExtraida",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_PaginaExtraida_ServentiaId_OperacaoId",
            table: "PaginaExtraida",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_PaginaExtraida_ServentiaId_OperacaoId_ExtracaoId",
            table: "PaginaExtraida",
            columns: new[] { "ServentiaId", "OperacaoId", "ExtracaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_PaginaExtraida_ServentiaId_OperacaoId_PaginaFonteId",
            table: "PaginaExtraida",
            columns: new[] { "ServentiaId", "OperacaoId", "PaginaFonteId" });

        migrationBuilder.CreateIndex(
            name: "IX_PaginaFonte_ServentiaId",
            table: "PaginaFonte",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_PaginaFonte_ServentiaId_OperacaoId",
            table: "PaginaFonte",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_PaginaFonte_ServentiaId_OperacaoId_FonteId_Numero",
            table: "PaginaFonte",
            columns: new[] { "ServentiaId", "OperacaoId", "FonteId", "Numero" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ReconstrucaoEvento_ServentiaId_OperacaoId_EventoId",
            table: "ReconstrucaoEvento",
            columns: new[] { "ServentiaId", "OperacaoId", "EventoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAchado_ServentiaId",
            table: "RevisaoAchado",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAchado_ServentiaId_OperacaoId",
            table: "RevisaoAchado",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAchado_ServentiaId_OperacaoId_AchadoId",
            table: "RevisaoAchado",
            columns: new[] { "ServentiaId", "OperacaoId", "AchadoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAto_ServentiaId",
            table: "RevisaoAto",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAto_ServentiaId_OperacaoId",
            table: "RevisaoAto",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAto_ServentiaId_OperacaoId_AtoId_NumeroVersao",
            table: "RevisaoAto",
            columns: new[] { "ServentiaId", "OperacaoId", "AtoId", "NumeroVersao" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoAtoCampo_ServentiaId_OperacaoId_RevisaoCampoId",
            table: "RevisaoAtoCampo",
            columns: new[] { "ServentiaId", "OperacaoId", "RevisaoCampoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoCampo_ServentiaId",
            table: "RevisaoCampo",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoCampo_ServentiaId_OperacaoId",
            table: "RevisaoCampo",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_RevisaoCampo_ServentiaId_OperacaoId_CampoId_NumeroVersao",
            table: "RevisaoCampo",
            columns: new[] { "ServentiaId", "OperacaoId", "CampoId", "NumeroVersao" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_SegmentoAto_ServentiaId",
            table: "SegmentoAto",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_SegmentoAto_ServentiaId_OperacaoId",
            table: "SegmentoAto",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_SegmentoAto_ServentiaId_OperacaoId_ExtracaoId",
            table: "SegmentoAto",
            columns: new[] { "ServentiaId", "OperacaoId", "ExtracaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_SegmentoAtoEvidencia_ServentiaId_OperacaoId_EvidenciaId",
            table: "SegmentoAtoEvidencia",
            columns: new[] { "ServentiaId", "OperacaoId", "EvidenciaId" });

        migrationBuilder.CreateIndex(
            name: "IX_SnapshotFonte_ServentiaId_OperacaoId_FonteId",
            table: "SnapshotFonte",
            columns: new[] { "ServentiaId", "OperacaoId", "FonteId" });

        migrationBuilder.CreateIndex(
            name: "IX_TentativaJob_ServentiaId",
            table: "TentativaJob",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_TentativaJob_ServentiaId_OperacaoId",
            table: "TentativaJob",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_TentativaJob_ServentiaId_OperacaoId_JobId_Numero",
            table: "TentativaJob",
            columns: new[] { "ServentiaId", "OperacaoId", "JobId", "Numero" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TitularidadeSnapshot_ServentiaId",
            table: "TitularidadeSnapshot",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_TitularidadeSnapshot_ServentiaId_OperacaoId",
            table: "TitularidadeSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_TitularidadeSnapshot_ServentiaId_OperacaoId_SnapshotId",
            table: "TitularidadeSnapshot",
            columns: new[] { "ServentiaId", "OperacaoId", "SnapshotId" });

        migrationBuilder.CreateIndex(
            name: "IX_TransmissaoProposta_ServentiaId",
            table: "TransmissaoProposta",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_TransmissaoProposta_ServentiaId_OperacaoId",
            table: "TransmissaoProposta",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_TransmissaoProposta_ServentiaId_OperacaoId_CenarioRevisaoId",
            table: "TransmissaoProposta",
            columns: new[] { "ServentiaId", "OperacaoId", "CenarioRevisaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_UsuarioServentia_ServentiaId",
            table: "UsuarioServentia",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ValidadeExecucao_ServentiaId",
            table: "ValidadeExecucao",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_ValidadeExecucao_ServentiaId_OperacaoId",
            table: "ValidadeExecucao",
            columns: new[] { "ServentiaId", "OperacaoId" });

        migrationBuilder.CreateIndex(
            name: "IX_VinculoAtosFontes_ServentiaId",
            table: "VinculoAtosFontes",
            column: "ServentiaId");

        migrationBuilder.CreateIndex(
            name: "IX_VinculoAtosFontes_ServentiaId_OperacaoId",
            table: "VinculoAtosFontes",
            columns: new[] { "ServentiaId", "OperacaoId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AchadoEvidencia");

        migrationBuilder.DropTable(
            name: "AprovacaoSnapshot");

        migrationBuilder.DropTable(
            name: "CampoCandidatoEvidencia");

        migrationBuilder.DropTable(
            name: "DireitoEvidencia");

        migrationBuilder.DropTable(
            name: "DireitoTitular");

        migrationBuilder.DropTable(
            name: "DossieEmitido");

        migrationBuilder.DropTable(
            name: "EventoAuditoria");

        migrationBuilder.DropTable(
            name: "EventoRevisadoAlvo");

        migrationBuilder.DropTable(
            name: "LacunaReconstrucao");

        migrationBuilder.DropTable(
            name: "MatriculaReferencia");

        migrationBuilder.DropTable(
            name: "PacoteRegra");

        migrationBuilder.DropTable(
            name: "ReconstrucaoEvento");

        migrationBuilder.DropTable(
            name: "RevisaoAchado");

        migrationBuilder.DropTable(
            name: "RevisaoAtoCampo");

        migrationBuilder.DropTable(
            name: "SegmentoAtoEvidencia");

        migrationBuilder.DropTable(
            name: "SnapshotFonte");

        migrationBuilder.DropTable(
            name: "TentativaJob");

        migrationBuilder.DropTable(
            name: "TitularidadeEvidencia");

        migrationBuilder.DropTable(
            name: "TitularidadeSnapshot");

        migrationBuilder.DropTable(
            name: "TransmissaoProposta");

        migrationBuilder.DropTable(
            name: "UsuarioServentia");

        migrationBuilder.DropTable(
            name: "ValidadeExecucao");

        migrationBuilder.DropTable(
            name: "VinculoAtosFontes");

        migrationBuilder.DropTable(
            name: "DireitoDocumentado");

        migrationBuilder.DropTable(
            name: "EventoRevisado");

        migrationBuilder.DropTable(
            name: "Achado");

        migrationBuilder.DropTable(
            name: "RevisaoCampo");

        migrationBuilder.DropTable(
            name: "EvidenciaFonte");

        migrationBuilder.DropTable(
            name: "JobProcessamento");

        migrationBuilder.DropTable(
            name: "Usuario");

        migrationBuilder.DropTable(
            name: "RevisaoAto");

        migrationBuilder.DropTable(
            name: "ExecucaoSimulacao");

        migrationBuilder.DropTable(
            name: "CampoCandidato");

        migrationBuilder.DropTable(
            name: "PaginaExtraida");

        migrationBuilder.DropTable(
            name: "CenarioRevisao");

        migrationBuilder.DropTable(
            name: "AtoCandidato");

        migrationBuilder.DropTable(
            name: "PaginaFonte");

        migrationBuilder.DropTable(
            name: "Cenario");

        migrationBuilder.DropTable(
            name: "MatriculaSnapshot");

        migrationBuilder.DropTable(
            name: "ExecucaoInterpretacao");

        migrationBuilder.DropTable(
            name: "ExecucaoReconstrucao");

        migrationBuilder.DropTable(
            name: "SegmentoAto");

        migrationBuilder.DropTable(
            name: "ExecucaoExtracao");

        migrationBuilder.DropTable(
            name: "FonteDocumental");

        migrationBuilder.DropTable(
            name: "Operacao");

        migrationBuilder.DropTable(
            name: "Serventia");
    }
}
