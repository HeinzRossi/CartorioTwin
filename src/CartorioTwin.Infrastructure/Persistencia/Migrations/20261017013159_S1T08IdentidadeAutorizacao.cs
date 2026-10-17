using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartorioTwin.Infrastructure.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class S1T08IdentidadeAutorizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdentidadeExterna",
                table: "Usuario",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmissorIdentidade",
                table: "Usuario",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutorizacaoRevisao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServentiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoRecurso = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RecursoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersaoAlvo = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerfilEfetivo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Capacidade = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmitidaEmUtc = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorizacaoRevisao", x => x.Id);
                    table.UniqueConstraint("AK_AutorizacaoRevisao_ServentiaId_Id", x => new { x.ServentiaId, x.Id });
                    table.UniqueConstraint("AK_AutorizacaoRevisao_ServentiaId_OperacaoId_Id", x => new { x.ServentiaId, x.OperacaoId, x.Id });
                    table.CheckConstraint("CK_AutorizacaoRevisao_Id_NaoVazio", "[Id] <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("CK_AutorizacaoRevisao_OperacaoId_NaoVazio", "[OperacaoId] <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("CK_AutorizacaoRevisao_ServentiaId_NaoVazio", "[ServentiaId] <> '00000000-0000-0000-0000-000000000000'");
                    table.CheckConstraint("CK_AutorizacaoRevisao_Valores", "[VersaoAlvo] > 0 AND DATEPART(TZOFFSET, [EmitidaEmUtc]) = 0 AND [UsuarioId] <> '00000000-0000-0000-0000-000000000000' AND [RecursoId] <> '00000000-0000-0000-0000-000000000000' AND [CorrelationId] <> '00000000-0000-0000-0000-000000000000'");
                    table.ForeignKey(
                        name: "FK_AutorizacaoRevisao_Operacao_ServentiaId_OperacaoId",
                        columns: x => new { x.ServentiaId, x.OperacaoId },
                        principalTable: "Operacao",
                        principalColumns: new[] { "ServentiaId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutorizacaoRevisao_Serventia_ServentiaId",
                        column: x => x.ServentiaId,
                        principalTable: "Serventia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutorizacaoRevisao_UsuarioServentia_UsuarioId_ServentiaId",
                        columns: x => new { x.UsuarioId, x.ServentiaId },
                        principalTable: "UsuarioServentia",
                        principalColumns: new[] { "UsuarioId", "ServentiaId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_UsuarioServentia_Perfil",
                table: "UsuarioServentia",
                sql: "[Perfil] IN ('Operador', 'Conferente', 'ResponsavelJuridico', 'Administrador', 'Auditor')");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_EmissorIdentidade_IdentidadeExterna",
                table: "Usuario",
                columns: new[] { "EmissorIdentidade", "IdentidadeExterna" },
                unique: true,
                filter: "[EmissorIdentidade] IS NOT NULL AND [IdentidadeExterna] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Usuario_IdentidadeExterna",
                table: "Usuario",
                sql: "([EmissorIdentidade] IS NULL AND [IdentidadeExterna] IS NULL) OR ([EmissorIdentidade] IS NOT NULL AND [EmissorIdentidade] <> '' AND [IdentidadeExterna] IS NOT NULL AND [IdentidadeExterna] <> '')");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacaoRevisao_ServentiaId",
                table: "AutorizacaoRevisao",
                column: "ServentiaId");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacaoRevisao_ServentiaId_OperacaoId",
                table: "AutorizacaoRevisao",
                columns: new[] { "ServentiaId", "OperacaoId" });

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacaoRevisao_UsuarioId_ServentiaId",
                table: "AutorizacaoRevisao",
                columns: new[] { "UsuarioId", "ServentiaId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutorizacaoRevisao");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UsuarioServentia_Perfil",
                table: "UsuarioServentia");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_EmissorIdentidade_IdentidadeExterna",
                table: "Usuario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Usuario_IdentidadeExterna",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "EmissorIdentidade",
                table: "Usuario");

            migrationBuilder.AlterColumn<string>(
                name: "IdentidadeExterna",
                table: "Usuario",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);
        }
    }
}
