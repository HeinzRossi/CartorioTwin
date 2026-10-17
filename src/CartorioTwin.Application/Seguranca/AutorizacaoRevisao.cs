using CartorioTwin.Domain.Documentos;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Application.Seguranca;

public sealed record RegistroAutorizacaoRevisao(
    AutorizacaoRevisaoId Id,
    ServentiaId ServentiaId,
    Guid OperacaoId,
    string TipoRecurso,
    Guid RecursoId,
    long VersaoAlvo,
    UsuarioId UsuarioId,
    PerfilAcesso PerfilEfetivo,
    string Capacidade,
    DateTimeOffset EmitidaEmUtc,
    Guid CorrelationId);

public interface IRepositorioAutorizacoesRevisao
{
    Task AdicionarAsync(RegistroAutorizacaoRevisao registro, CancellationToken cancellationToken);
}

public sealed class EmissorAutorizacaoRevisao(IRepositorioAutorizacoesRevisao repositorio)
{
    public async Task<RegistroAutorizacaoRevisao> EmitirAsync(
        ContextoAcesso contexto,
        AutorizacaoRevisaoId id,
        Guid operacaoId,
        string tipoRecurso,
        Guid recursoId,
        long versaoAlvo,
        DateTimeOffset emitidaEmUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        if (contexto.TipoAtor != TipoAtorAcesso.Usuario || !contexto.Possui(CapacidadesAcesso.FatosRevisar))
        {
            throw new UnauthorizedAccessException("Somente usuário autorizado pode emitir autorização de revisão.");
        }

        if (id == default || operacaoId == Guid.Empty || recursoId == Guid.Empty || correlationId == Guid.Empty)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Os identificadores da autorização devem ser válidos.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tipoRecurso);
        if (tipoRecurso.Any(char.IsControl) || versaoAlvo <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tipoRecurso));
        }

        if (emitidaEmUtc == default || emitidaEmUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(emitidaEmUtc), "O instante deve ser UTC explícito.");
        }

        var registro = new RegistroAutorizacaoRevisao(
            id,
            contexto.ServentiaId!.Value,
            operacaoId,
            tipoRecurso.Trim(),
            recursoId,
            versaoAlvo,
            contexto.UsuarioId!.Value,
            contexto.Perfil!.Value,
            CapacidadesAcesso.FatosRevisar,
            emitidaEmUtc,
            correlationId);

        await repositorio.AdicionarAsync(registro, cancellationToken);
        return registro;
    }
}
