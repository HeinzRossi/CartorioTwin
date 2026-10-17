using Microsoft.EntityFrameworkCore;

namespace CartorioTwin.Infrastructure.Persistencia;

public sealed class CartorioTwinDbContext(DbContextOptions<CartorioTwinDbContext> options)
    : DbContext(options)
{
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ValidarRegistrosAppendOnly();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ValidarRegistrosAppendOnly();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.AplicarCatalogoCartorioTwin();
    }

    private void ValidarRegistrosAppendOnly()
    {
        if (ChangeTracker.Entries<AutorizacaoRevisaoRegistro>()
            .Any(x => x.State is EntityState.Modified or EntityState.Deleted))
        {
            throw new InvalidOperationException("Autorizações de revisão são registros append-only.");
        }
    }
}
