using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Application.Seguranca;

public readonly record struct ReferenciaStorage
{
    private ReferenciaStorage(ServentiaId serventiaId, Guid operacaoId, string chave)
    {
        ServentiaId = serventiaId;
        OperacaoId = operacaoId;
        Chave = chave;
    }

    public ServentiaId ServentiaId { get; }
    public Guid OperacaoId { get; }
    public string Chave { get; }

    public static ReferenciaStorage Gerar(
        ServentiaId serventiaId,
        Guid operacaoId,
        Guid objetoId,
        string etapa)
    {
        if (serventiaId == default || operacaoId == Guid.Empty || objetoId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(operacaoId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(etapa);
        if (etapa.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-'))
        {
            throw new ArgumentException("A etapa deve usar apenas ASCII alfanumérico ou hífen.", nameof(etapa));
        }

        var chave = $"{serventiaId.Valor:N}/{operacaoId:N}/{etapa}/{objetoId:N}";
        return new ReferenciaStorage(serventiaId, operacaoId, chave);
    }
}

public interface IStorageObjetos
{
    Task<Stream> AbrirLeituraAsync(string chave, CancellationToken cancellationToken);
    Task GravarStagingAsync(string chave, Stream conteudo, CancellationToken cancellationToken);
    Task PromoverAsync(string chaveStaging, string chaveFinal, CancellationToken cancellationToken);
}

public sealed class StorageDocumentalAutorizado(IStorageObjetos storage)
{
    public Task<Stream> AbrirLeituraAsync(
        ContextoAcesso contexto,
        ReferenciaStorage referencia,
        CancellationToken cancellationToken = default)
    {
        Validar(contexto, referencia, CapacidadesAcesso.FontesLer);
        return storage.AbrirLeituraAsync(referencia.Chave, cancellationToken);
    }

    public Task GravarStagingAsync(
        ContextoAcesso contexto,
        ReferenciaStorage referencia,
        Stream conteudo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(conteudo);
        Validar(contexto, referencia, CapacidadesAcesso.FontesCriar);
        return storage.GravarStagingAsync(referencia.Chave, conteudo, cancellationToken);
    }

    public Task PromoverAsync(
        ContextoAcesso contexto,
        ReferenciaStorage staging,
        ReferenciaStorage destino,
        CancellationToken cancellationToken = default)
    {
        Validar(contexto, staging, CapacidadesAcesso.FontesCriar);
        Validar(contexto, destino, CapacidadesAcesso.FontesCriar);
        if (staging.OperacaoId != destino.OperacaoId)
        {
            throw new UnauthorizedAccessException("A promoção não pode atravessar operações.");
        }

        return storage.PromoverAsync(staging.Chave, destino.Chave, cancellationToken);
    }

    private static void Validar(ContextoAcesso contexto, ReferenciaStorage referencia, string capacidade)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        if (contexto.TipoAtor != TipoAtorAcesso.Usuario
            || contexto.ServentiaId != referencia.ServentiaId
            || !contexto.Possui(capacidade))
        {
            throw new UnauthorizedAccessException("Acesso ao storage negado.");
        }
    }
}
