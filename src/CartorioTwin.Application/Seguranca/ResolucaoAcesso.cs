using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Application.Seguranca;

public sealed record VinculoUsuarioAcesso(
    UsuarioId UsuarioId,
    ServentiaId ServentiaId,
    PerfilAcesso Perfil,
    bool UsuarioAtivo,
    bool ServentiaAtiva,
    bool VinculoAtivo);

public interface IRepositorioVinculosAcesso
{
    Task<VinculoUsuarioAcesso?> BuscarAsync(
        string emissor,
        string subject,
        ServentiaId serventiaId,
        CancellationToken cancellationToken);
}

public enum FalhaResolucaoAcesso
{
    Nenhuma = 0,
    IdentidadeInvalida = 1,
    VinculoNaoEncontrado = 2,
    VinculoInativo = 3,
}

public sealed record ResultadoResolucaoAcesso(ContextoAcesso? Contexto, FalhaResolucaoAcesso Falha)
{
    public bool Sucesso => Contexto is not null && Falha == FalhaResolucaoAcesso.Nenhuma;
}

public sealed class ResolvedorAcesso(IRepositorioVinculosAcesso repositorio)
{
    public async Task<ResultadoResolucaoAcesso> ResolverUsuarioAsync(
        string emissor,
        string subject,
        ServentiaId serventiaId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emissor) || string.IsNullOrWhiteSpace(subject) || serventiaId == default)
        {
            return new(null, FalhaResolucaoAcesso.IdentidadeInvalida);
        }

        var vinculo = await repositorio.BuscarAsync(emissor, subject, serventiaId, cancellationToken);
        if (vinculo is null)
        {
            return new(null, FalhaResolucaoAcesso.VinculoNaoEncontrado);
        }

        if (!vinculo.UsuarioAtivo || !vinculo.ServentiaAtiva || !vinculo.VinculoAtivo)
        {
            return new(null, FalhaResolucaoAcesso.VinculoInativo);
        }

        return new(
            ContextoAcesso.Usuario($"{emissor}|{subject}", vinculo.ServentiaId, vinculo.UsuarioId, vinculo.Perfil),
            FalhaResolucaoAcesso.Nenhuma);
    }
}
