namespace CartorioTwin.Infrastructure.Persistencia;

internal interface IRegistroTenant
{
    Guid Id { get; set; }
    Guid ServentiaId { get; set; }
}

internal interface IRegistroOperacao : IRegistroTenant
{
    Guid OperacaoId { get; set; }
}

internal sealed class ServentiaRegistro
{
    public Guid Id { get; set; }
    public string Uf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class UsuarioRegistro
{
    public Guid Id { get; set; }
    public string? EmissorIdentidade { get; set; }
    public string? IdentidadeExterna { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

internal sealed class AutorizacaoRevisaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public string TipoRecurso { get; set; } = string.Empty;
    public Guid RecursoId { get; set; }
    public long VersaoAlvo { get; set; }
    public Guid UsuarioId { get; set; }
    public string PerfilEfetivo { get; set; } = string.Empty;
    public string Capacidade { get; set; } = string.Empty;
    public DateTimeOffset EmitidaEmUtc { get; set; }
    public Guid CorrelationId { get; set; }
}

internal sealed class UsuarioServentiaRegistro
{
    public Guid UsuarioId { get; set; }
    public Guid ServentiaId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

internal sealed class OperacaoRegistro : IRegistroTenant
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid MatriculaId { get; set; }
    public long RevisaoAtual { get; set; }
    public string EstadoWorkflow { get; set; } = string.Empty;
    public DateTimeOffset CriadaEmUtc { get; set; }
    public DateTimeOffset AtualizadaEmUtc { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

internal sealed class MatriculaReferenciaRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public string ServentiaOrigem { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Cnm { get; set; }
    public bool IdentificacaoRevisada { get; set; }
}

internal sealed class FonteDocumentalRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public int Versao { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string MimeValidado { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public DateOnly? DataFonte { get; set; }
    public string? Emissor { get; set; }
    public string EstadoTecnico { get; set; } = string.Empty;
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class PaginaFonteRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid FonteId { get; set; }
    public int Numero { get; set; }
    public string? DimensoesJson { get; set; }
    public short Rotacao { get; set; }
}

internal sealed class ExecucaoExtracaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid FonteId { get; set; }
    public int RevisaoConfiguracao { get; set; }
    public string VersoesMotoresJson { get; set; } = "{}";
    public string Estado { get; set; } = string.Empty;
    public string ManifestoJson { get; set; } = "{}";
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class PaginaExtraidaRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExtracaoId { get; set; }
    public Guid PaginaFonteId { get; set; }
    public string Estrategia { get; set; } = string.Empty;
    public string TextoStorageKey { get; set; } = string.Empty;
    public string? LayoutStorageKey { get; set; }
    public string TextoHash { get; set; } = string.Empty;
    public decimal? Qualidade { get; set; }
}

internal sealed class EvidenciaFonteRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExtracaoId { get; set; }
    public Guid PaginaExtraidaId { get; set; }
    public int InicioUtf16 { get; set; }
    public int FimUtf16 { get; set; }
    public string Trecho { get; set; } = string.Empty;
    public string? BoundingBoxJson { get; set; }
    public string? RevisaoVisualJson { get; set; }
}

internal sealed class SegmentoAtoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExtracaoId { get; set; }
    public int OrdemNaFonte { get; set; }
    public string? RotuloLocalizado { get; set; }
}

internal sealed class SegmentoAtoEvidenciaRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid SegmentoId { get; set; }
    public Guid EvidenciaId { get; set; }
}

internal sealed class ExecucaoInterpretacaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid SegmentoId { get; set; }
    public string ProvedorVersao { get; set; } = string.Empty;
    public string PromptHash { get; set; } = string.Empty;
    public string SchemaVersao { get; set; } = string.Empty;
    public string ConfiguracaoHash { get; set; } = string.Empty;
    public string SaidaStorageKey { get; set; } = string.Empty;
    public string IssuesJson { get; set; } = "[]";
}

internal sealed class AtoCandidatoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid MatriculaId { get; set; }
    public Guid InterpretacaoId { get; set; }
    public string? IdentificadorSugerido { get; set; }
    public string TipoSugerido { get; set; } = string.Empty;
    public int OrdemDocumental { get; set; }
    public string DatasSegregadasJson { get; set; } = "{}";
    public string IssuesJson { get; set; } = "[]";
}

internal sealed class CampoCandidatoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid AtoCandidatoId { get; set; }
    public string CaminhoCampo { get; set; } = string.Empty;
    public string? ValorSugeridoJson { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

internal sealed class CampoCandidatoEvidenciaRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid CampoId { get; set; }
    public Guid EvidenciaId { get; set; }
}

internal sealed class RevisaoCampoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid CampoId { get; set; }
    public int NumeroVersao { get; set; }
    public string? ValorFinalJson { get; set; }
    public string Decisao { get; set; } = string.Empty;
    public string? Justificativa { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid AutorizacaoId { get; set; }
    public DateTimeOffset EmUtc { get; set; }
}

internal sealed class RevisaoAtoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid AtoId { get; set; }
    public int NumeroVersao { get; set; }
    public string Decisao { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public DateTimeOffset EmUtc { get; set; }
    public string Hash { get; set; } = string.Empty;
}

internal sealed class RevisaoAtoCampoRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid RevisaoAtoId { get; set; }
    public Guid RevisaoCampoId { get; set; }
}

internal sealed class VinculoAtosFontesRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid MatriculaId { get; set; }
    public Guid AtoOrigemId { get; set; }
    public Guid AtoRelacionadoId { get; set; }
    public string TipoRelacao { get; set; } = string.Empty;
    public Guid RevisaoId { get; set; }
}

internal sealed class EventoRevisadoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid RevisaoAtoId { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public long? FracaoNumerador { get; set; }
    public long? FracaoDenominador { get; set; }
    public int OrdemRevisada { get; set; }
    public string MapeamentoVersao { get; set; } = string.Empty;
    public string PremissasJson { get; set; } = "[]";
    public string Hash { get; set; } = string.Empty;
}

internal sealed class EventoRevisadoAlvoRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid EventoId { get; set; }
    public Guid PessoaId { get; set; }
    public string Papel { get; set; } = string.Empty;
}

internal sealed class ExecucaoReconstrucaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public long RevisaoCapturada { get; set; }
    public string ManifestoHash { get; set; } = string.Empty;
    public string MotorVersao { get; set; } = string.Empty;
    public string SaidaStorageKey { get; set; } = string.Empty;
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class ReconstrucaoEventoRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ReconstrucaoId { get; set; }
    public Guid EventoId { get; set; }
}

internal sealed class LacunaReconstrucaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ReconstrucaoId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Relevancia { get; set; } = string.Empty;
    public string ReferenciasJson { get; set; } = "[]";
    public string Motivo { get; set; } = string.Empty;
}

internal sealed class MatriculaSnapshotRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ReconstrucaoId { get; set; }
    public int NumeroVersao { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string Completude { get; set; } = string.Empty;
    public string ManifestoHash { get; set; } = string.Empty;
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class SnapshotFonteRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid SnapshotId { get; set; }
    public Guid FonteId { get; set; }
    public string Finalidade { get; set; } = string.Empty;
}

internal sealed class TitularidadeSnapshotRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid SnapshotId { get; set; }
    public Guid PessoaId { get; set; }
    public string TipoDireito { get; set; } = string.Empty;
    public long Numerador { get; set; }
    public long Denominador { get; set; }
}

internal sealed class TitularidadeEvidenciaRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid TitularidadeId { get; set; }
    public Guid EvidenciaId { get; set; }
    public Guid EventoId { get; set; }
}

internal sealed class AprovacaoSnapshotRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid SnapshotId { get; set; }
    public string SnapshotHash { get; set; } = string.Empty;
    public long RevisaoOperacao { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTimeOffset EmUtc { get; set; }
    public string PremissasJson { get; set; } = "[]";
    public string Justificativa { get; set; } = string.Empty;
}

internal sealed class DireitoDocumentadoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid AtoId { get; set; }
    public Guid MatriculaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string TipoExtensao { get; set; } = string.Empty;
    public long? Numerador { get; set; }
    public long? Denominador { get; set; }
    public string ConhecimentoEfeito { get; set; } = string.Empty;
    public DateOnly? InicioDocumentado { get; set; }
    public DateOnly? FimDocumentado { get; set; }
}

internal sealed class DireitoTitularRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid DireitoId { get; set; }
    public Guid PessoaId { get; set; }
}

internal sealed class DireitoEvidenciaRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid DireitoId { get; set; }
    public Guid EvidenciaId { get; set; }
}

internal sealed class CenarioRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid? RevisaoAtualId { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

internal sealed class CenarioRevisaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid CenarioId { get; set; }
    public int NumeroVersao { get; set; }
    public Guid SnapshotId { get; set; }
    public string PropostaHash { get; set; } = string.Empty;
}

internal sealed class TransmissaoPropostaRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid CenarioRevisaoId { get; set; }
    public Guid MatriculaId { get; set; }
    public Guid AlienanteId { get; set; }
    public Guid AdquirenteId { get; set; }
    public string TipoDireito { get; set; } = string.Empty;
    public string BaseFracao { get; set; } = string.Empty;
    public long Numerador { get; set; }
    public long Denominador { get; set; }
}

internal sealed class ExecucaoSimulacaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid CenarioRevisaoId { get; set; }
    public Guid SnapshotId { get; set; }
    public string ManifestoHash { get; set; } = string.Empty;
    public string EntradasStorageKey { get; set; } = string.Empty;
    public string ResultadoStorageKey { get; set; } = string.Empty;
    public string MotorVersao { get; set; } = string.Empty;
    public string Conclusao { get; set; } = string.Empty;
    public string Completude { get; set; } = string.Empty;
    public string Cobertura { get; set; } = string.Empty;
}

internal sealed class ValidadeExecucaoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExecucaoId { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public Guid? RevisaoSubstitutaId { get; set; }
    public DateTimeOffset AtualizadaEmUtc { get; set; }
}

internal sealed class AchadoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExecucaoId { get; set; }
    public string RegraCodigo { get; set; } = string.Empty;
    public string RegraVersao { get; set; } = string.Empty;
    public string Severidade { get; set; } = string.Empty;
    public string FatosComparadosJson { get; set; } = "[]";
    public string Mensagem { get; set; } = string.Empty;
}

internal sealed class AchadoEvidenciaRegistro
{
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid AchadoId { get; set; }
    public Guid EvidenciaId { get; set; }
}

internal sealed class RevisaoAchadoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid AchadoId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Decisao { get; set; } = string.Empty;
    public string Justificativa { get; set; } = string.Empty;
    public DateTimeOffset EmUtc { get; set; }
}

internal sealed class DossieEmitidoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid ExecucaoId { get; set; }
    public string ManifestoHash { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string ConteudoHash { get; set; } = string.Empty;
    public DateTimeOffset EmitidoEmUtc { get; set; }
}

internal sealed class PacoteRegraRegistro : IRegistroTenant
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Versao { get; set; } = string.Empty;
    public string ConteudoHash { get; set; } = string.Empty;
    public string? Jurisdicao { get; set; }
    public DateOnly? VigenciaInicio { get; set; }
    public DateOnly? VigenciaFim { get; set; }
    public string AprovacoesJson { get; set; } = "[]";
    public string Situacao { get; set; } = string.Empty;
}

internal sealed class JobProcessamentoRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid? FonteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string ManifestoHash { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTimeOffset? ProximaExecucaoEmUtc { get; set; }
    public DateTimeOffset? LeaseAteUtc { get; set; }
    public string? WorkerId { get; set; }
    public long FencingToken { get; set; }
    public DateTimeOffset CriadaEmUtc { get; set; }
}

internal sealed class TentativaJobRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid JobId { get; set; }
    public int Numero { get; set; }
    public DateTimeOffset InicioUtc { get; set; }
    public DateTimeOffset? FimUtc { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? ErroCodigo { get; set; }
    public string ArtefatosJson { get; set; } = "[]";
    public string ConfiguracaoFixadaJson { get; set; } = "{}";
}

internal sealed class EventoAuditoriaRegistro : IRegistroOperacao
{
    public Guid Id { get; set; }
    public Guid ServentiaId { get; set; }
    public Guid OperacaoId { get; set; }
    public Guid? AtorId { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string Recurso { get; set; } = string.Empty;
    public Guid RecursoId { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTimeOffset EmUtc { get; set; }
}
