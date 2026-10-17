using Microsoft.EntityFrameworkCore;

namespace CartorioTwin.Infrastructure.Persistencia;

public sealed class CartorioTwinDbContext(DbContextOptions<CartorioTwinDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.AplicarCatalogoCartorioTwin();
    }
}
