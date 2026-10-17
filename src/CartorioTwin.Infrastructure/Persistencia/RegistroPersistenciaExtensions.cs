using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CartorioTwin.Application.Seguranca;

namespace CartorioTwin.Infrastructure.Persistencia;

public static class RegistroPersistenciaExtensions
{
    public static IServiceCollection AddPersistenciaCartorioTwin(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<CartorioTwinDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(CartorioTwinDbContext).Assembly.FullName)));
        services.AddScoped<IRepositorioVinculosAcesso, RepositorioVinculosAcesso>();
        services.AddScoped<IRepositorioAutorizacoesRevisao, RepositorioAutorizacoesRevisao>();
        services.AddScoped<ResolvedorAcesso>();
        services.AddScoped<EmissorAutorizacaoRevisao>();
        return services;
    }
}
