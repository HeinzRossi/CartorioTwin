using System.Collections.Frozen;
using CartorioTwin.Domain.Identidades;

namespace CartorioTwin.Application.Seguranca;

public enum TipoAtorAcesso
{
    Usuario = 1,
    Servico = 2,
}

public enum PerfilAcesso
{
    Operador = 1,
    Conferente = 2,
    ResponsavelJuridico = 3,
    Administrador = 4,
    Auditor = 5,
}

public static class CapacidadesAcesso
{
    public const string FontesLer = "fontes.ler";
    public const string FontesCriar = "fontes.criar";
    public const string FatosRevisar = "fatos.revisar";
    public const string SnapshotsRevisar = "snapshots.revisar";
    public const string SimulacoesExecutar = "simulacoes.executar";
    public const string RevisoesJuridicasRegistrar = "revisoes-juridicas.registrar";
    public const string DossiesExportar = "dossies.exportar";
    public const string UsuariosAdministrar = "usuarios.administrar";
    public const string AuditoriaConsultar = "auditoria.consultar";
    public const string JobsProcessar = "jobs.processar";
}

public static class MatrizCapacidadesAcesso
{
    private static readonly FrozenDictionary<PerfilAcesso, FrozenSet<string>> PorPerfil =
        new Dictionary<PerfilAcesso, FrozenSet<string>>
        {
            [PerfilAcesso.Operador] = Conjunto(CapacidadesAcesso.FontesLer, CapacidadesAcesso.FontesCriar),
            [PerfilAcesso.Conferente] = Conjunto(
                CapacidadesAcesso.FontesLer,
                CapacidadesAcesso.FontesCriar,
                CapacidadesAcesso.FatosRevisar,
                CapacidadesAcesso.SnapshotsRevisar,
                CapacidadesAcesso.SimulacoesExecutar,
                CapacidadesAcesso.DossiesExportar),
            [PerfilAcesso.ResponsavelJuridico] = Conjunto(
                CapacidadesAcesso.FontesLer,
                CapacidadesAcesso.FontesCriar,
                CapacidadesAcesso.FatosRevisar,
                CapacidadesAcesso.SnapshotsRevisar,
                CapacidadesAcesso.SimulacoesExecutar,
                CapacidadesAcesso.RevisoesJuridicasRegistrar,
                CapacidadesAcesso.DossiesExportar,
                CapacidadesAcesso.AuditoriaConsultar),
            [PerfilAcesso.Administrador] = Conjunto(
                CapacidadesAcesso.UsuariosAdministrar,
                CapacidadesAcesso.AuditoriaConsultar),
            [PerfilAcesso.Auditor] = Conjunto(
                CapacidadesAcesso.DossiesExportar,
                CapacidadesAcesso.AuditoriaConsultar),
        }.ToFrozenDictionary();

    public static IReadOnlySet<string> Obter(PerfilAcesso perfil)
    {
        if (!PorPerfil.TryGetValue(perfil, out var capacidades))
        {
            throw new ArgumentOutOfRangeException(nameof(perfil));
        }

        return capacidades;
    }

    private static FrozenSet<string> Conjunto(params string[] capacidades) =>
        capacidades.ToFrozenSet(StringComparer.Ordinal);
}

public sealed record ContextoAcesso
{
    private ContextoAcesso(
        TipoAtorAcesso tipoAtor,
        string identidade,
        ServentiaId? serventiaId,
        UsuarioId? usuarioId,
        PerfilAcesso? perfil,
        IReadOnlySet<string> capacidades)
    {
        TipoAtor = tipoAtor;
        Identidade = identidade;
        ServentiaId = serventiaId;
        UsuarioId = usuarioId;
        Perfil = perfil;
        Capacidades = capacidades;
    }

    public TipoAtorAcesso TipoAtor { get; }
    public string Identidade { get; }
    public ServentiaId? ServentiaId { get; }
    public UsuarioId? UsuarioId { get; }
    public PerfilAcesso? Perfil { get; }
    public IReadOnlySet<string> Capacidades { get; }

    public bool Possui(string capacidade) => Capacidades.Contains(capacidade);

    public static ContextoAcesso Usuario(
        string identidade,
        ServentiaId serventiaId,
        UsuarioId usuarioId,
        PerfilAcesso perfil) =>
        new(
            TipoAtorAcesso.Usuario,
            ValidarIdentidade(identidade),
            serventiaId == default ? throw new ArgumentOutOfRangeException(nameof(serventiaId)) : serventiaId,
            usuarioId == default ? throw new ArgumentOutOfRangeException(nameof(usuarioId)) : usuarioId,
            Enum.IsDefined(perfil) ? perfil : throw new ArgumentOutOfRangeException(nameof(perfil)),
            MatrizCapacidadesAcesso.Obter(perfil));

    public static ContextoAcesso Servico(string clientId, bool workerAutorizado) =>
        new(
            TipoAtorAcesso.Servico,
            ValidarIdentidade(clientId),
            null,
            null,
            null,
            workerAutorizado
                ? new[] { CapacidadesAcesso.JobsProcessar }.ToFrozenSet(StringComparer.Ordinal)
                : FrozenSet<string>.Empty);

    private static string ValidarIdentidade(string identidade)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identidade);
        if (identidade.Any(char.IsControl))
        {
            throw new ArgumentException("A identidade não pode conter caracteres de controle.", nameof(identidade));
        }

        return identidade.Trim();
    }
}
