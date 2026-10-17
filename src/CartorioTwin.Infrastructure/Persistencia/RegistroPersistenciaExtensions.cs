using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CartorioTwin.Infrastructure.Persistencia;

public static class RegistroPersistenciaExtensions
{
    public static IServiceCollection AddPersistenciaCartorioTwin(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return services.AddDbContext<CartorioTwinDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(CartorioTwinDbContext).Assembly.FullName)));
    }
}
