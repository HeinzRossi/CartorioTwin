namespace CartorioTwin.Contracts.Documentos.V1;

public sealed record PacoteDocumentalV1(
    string SchemaVersion,
    IReadOnlyList<FonteDocumentalContratoV1> Fontes,
    IReadOnlyList<PaginaTextoContratoV1> Paginas,
    IReadOnlyList<AtoRegistralContratoV1> Atos,
    IReadOnlyList<DireitoDocumentadoContratoV1> Direitos,
    IReadOnlyList<FatoDocumentalContratoV1> Fatos,
    IReadOnlyList<RevisaoFatoContratoV1> Revisoes)
{
    public const string VersaoSuportada = "1.0-draft";
}

public sealed record FonteDocumentalContratoV1(
    string Id,
    long Versao,
    string HashSha256,
    string NomeSanitizado,
    string MimeType,
    long TamanhoBytes);

public sealed record PaginaTextoContratoV1(
    string FonteId,
    long FonteVersao,
    int Numero,
    string Texto,
    string TextoHashSha256);

public sealed record EvidenciaContratoV1(
    string FonteId,
    long FonteVersao,
    string FonteHashSha256,
    int Pagina,
    string TextoHashSha256,
    int InicioUtf16,
    int FimUtf16,
    string Trecho);

public sealed record AtoRegistralContratoV1(
    string Id,
    string MatriculaId,
    string FonteId,
    long FonteVersao,
    IdentificadorAtoContratoV1 Identificador,
    long OrdemNaFonte,
    DatasDocumentaisAtoContratoV1 Datas,
    IReadOnlyList<int> Paginas,
    IReadOnlyList<ParteAtoContratoV1> Partes,
    IReadOnlyList<RelacaoAtoContratoV1> Relacoes);

public sealed record IdentificadorAtoContratoV1(
    TipoIdentificadorAtoContratoV1 Tipo,
    long? Numero,
    string TextoOriginal);

public enum TipoIdentificadorAtoContratoV1
{
    Registro,
    Averbacao,
    NaoIdentificado,
}

public sealed record DatasDocumentaisAtoContratoV1(
    string? DataDoTitulo,
    string? DataDoAto,
    string? DataDaApresentacao);

public sealed record ParteAtoContratoV1(string RotuloDocumental, string? PessoaId);

public sealed record RelacaoAtoContratoV1(string AtoRelacionadoId, TipoRelacaoAtoContratoV1 Tipo);

public enum TipoRelacaoAtoContratoV1
{
    Referencia,
    Retificacao,
    Cancelamento,
    Correspondencia,
    Desconhecida,
}

public sealed record DireitoDocumentadoContratoV1(
    string Id,
    string AtoOrigemId,
    TipoDireitoDocumentadoContratoV1 Tipo,
    ExtensaoDireitoContratoV1 Extensao,
    ConhecimentoEfeitoDireitoContratoV1 ConhecimentoEfeito,
    string? InicioDocumentado,
    string? FimDocumentado,
    IReadOnlyList<string> Titulares,
    IReadOnlyList<EvidenciaContratoV1> Evidencias);

public enum TipoDireitoDocumentadoContratoV1
{
    PropriedadePlena,
    NuaPropriedade,
    Usufruto,
    Onus,
    Garantia,
    NaoIdentificado,
}

public sealed record ExtensaoDireitoContratoV1(
    TipoExtensaoDireitoContratoV1 Tipo,
    FracaoContratoV1? FracaoDoTotal);

public enum TipoExtensaoDireitoContratoV1
{
    NaoIdentificada,
    Total,
    FracaoDoTotal,
}

public enum ConhecimentoEfeitoDireitoContratoV1
{
    NaoAvaliado,
    Conhecido,
    Desconhecido,
    NaoSuportado,
}

public sealed record FatoDocumentalContratoV1(
    string Id,
    long Versao,
    string? FatoAnteriorId,
    string AtoOrigemId,
    string CaminhoCampo,
    ValorFatoContratoV1? ValorSugerido,
    IReadOnlyList<EvidenciaContratoV1> Evidencias,
    EstadoRevisaoFatoContratoV1 Estado,
    SituacaoEvidenciaFatoContratoV1 SituacaoEvidencia,
    string? RevisaoId);

public enum EstadoRevisaoFatoContratoV1
{
    Extraido,
    EmRevisao,
    Confirmado,
    Contestado,
    NaoSuportado,
}

public enum SituacaoEvidenciaFatoContratoV1
{
    Localizada,
    NaoLocalizada,
    DeclaradaSemEvidencia,
}

public sealed record RevisaoFatoContratoV1(
    string Id,
    string FatoId,
    long VersaoFato,
    DecisaoRevisaoFatoContratoV1 Decisao,
    ValorFatoContratoV1? ValorFinal,
    string? Justificativa,
    string UsuarioId,
    string AutorizacaoId,
    string EmUtc);

public enum DecisaoRevisaoFatoContratoV1
{
    Confirmar,
    Corrigir,
    Contestar,
    MarcarNaoLocalizado,
    DeclararSemEvidencia,
    MarcarNaoSuportado,
}

public sealed record ValorFatoContratoV1(
    TipoValorFatoContratoV1 Tipo,
    string? Texto = null,
    long? Inteiro = null,
    bool? Booleano = null,
    FracaoContratoV1? Fracao = null,
    string? Identidade = null);

public enum TipoValorFatoContratoV1
{
    Texto,
    Inteiro,
    Booleano,
    Fracao,
    Identidade,
}

public sealed record FracaoContratoV1(long Numerador, long Denominador);
