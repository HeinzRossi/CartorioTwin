using CartorioTwin.Application.Seguranca;
using CartorioTwin.Domain.Identidades;
using Microsoft.EntityFrameworkCore;

namespace CartorioTwin.Infrastructure.Persistencia;

internal sealed class RepositorioVinculosAcesso(CartorioTwinDbContext dbContext) : IRepositorioVinculosAcesso
{
    public async Task<VinculoUsuarioAcesso?> BuscarAsync(
        string emissor,
        string subject,
        ServentiaId serventiaId,
        CancellationToken cancellationToken)
    {
        var consulta =
            from usuario in dbContext.Set<UsuarioRegistro>().AsNoTracking()
            join vinculo in dbContext.Set<UsuarioServentiaRegistro>().AsNoTracking()
                on usuario.Id equals vinculo.UsuarioId
            join serventia in dbContext.Set<ServentiaRegistro>().AsNoTracking()
                on vinculo.ServentiaId equals serventia.Id
            where usuario.EmissorIdentidade == emissor
                && usuario.IdentidadeExterna == subject
                && vinculo.ServentiaId == serventiaId.Valor
            select new
            {
                UsuarioId = usuario.Id,
                vinculo.ServentiaId,
                vinculo.Perfil,
                UsuarioAtivo = usuario.Ativo,
                ServentiaAtiva = serventia.Ativa,
                VinculoAtivo = vinculo.Ativo,
            };

        var registro = await consulta.SingleOrDefaultAsync(cancellationToken);
        if (registro is null || !Enum.TryParse<PerfilAcesso>(registro.Perfil, out var perfil))
        {
            return null;
        }

        return new VinculoUsuarioAcesso(
            UsuarioId.Criar(registro.UsuarioId),
            ServentiaId.Criar(registro.ServentiaId),
            perfil,
            registro.UsuarioAtivo,
            registro.ServentiaAtiva,
            registro.VinculoAtivo);
    }
}

internal sealed class RepositorioAutorizacoesRevisao(CartorioTwinDbContext dbContext)
    : IRepositorioAutorizacoesRevisao
{
    public async Task AdicionarAsync(
        RegistroAutorizacaoRevisao registro,
        CancellationToken cancellationToken)
    {
        dbContext.Add(new AutorizacaoRevisaoRegistro
        {
            Id = registro.Id.Valor,
            ServentiaId = registro.ServentiaId.Valor,
            OperacaoId = registro.OperacaoId,
            TipoRecurso = registro.TipoRecurso,
            RecursoId = registro.RecursoId,
            VersaoAlvo = registro.VersaoAlvo,
            UsuarioId = registro.UsuarioId.Valor,
            PerfilEfetivo = registro.PerfilEfetivo.ToString(),
            Capacidade = registro.Capacidade,
            EmitidaEmUtc = registro.EmitidaEmUtc,
            CorrelationId = registro.CorrelationId,
        });
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
