using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CartorioTwin.Infrastructure.Persistencia;

public sealed class CartorioTwinDbContextFactory : IDesignTimeDbContextFactory<CartorioTwinDbContext>
{
    public CartorioTwinDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("CARTORIOTWIN_DESIGNTIME_CONNECTION")
            ?? throw new InvalidOperationException(
                "Defina CARTORIOTWIN_DESIGNTIME_CONNECTION para executar comandos EF Core de design-time.");

        var options = new DbContextOptionsBuilder<CartorioTwinDbContext>()
            .UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(CartorioTwinDbContext).Assembly.FullName))
            .Options;

        return new CartorioTwinDbContext(options);
    }
}
