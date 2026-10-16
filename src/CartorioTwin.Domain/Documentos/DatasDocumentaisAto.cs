namespace CartorioTwin.Domain.Documentos;

public sealed record DatasDocumentaisAto(
    DateOnly? DataDoTitulo,
    DateOnly? DataDoAto,
    DateOnly? DataDaApresentacao)
{
    public static DatasDocumentaisAto Nenhuma { get; } = new(null, null, null);
}
