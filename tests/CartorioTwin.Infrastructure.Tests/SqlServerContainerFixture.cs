using Testcontainers.MsSql;

namespace CartorioTwin.Infrastructure.Tests;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private const string ImagemSqlServer =
        "mcr.microsoft.com/mssql/server@sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090";

    private MsSqlContainer? container;

    internal MsSqlContainer Container =>
        container ?? throw new InvalidOperationException("O container SQL Server não foi inicializado.");

    private bool iniciado;

    public async Task InitializeAsync()
    {
        if (string.Equals(
                Environment.GetEnvironmentVariable("CARTORIOTWIN_SKIP_SQLSERVER_TESTS"),
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        container = new MsSqlBuilder(ImagemSqlServer).Build();
        await container.StartAsync();
        iniciado = true;
    }

    public Task DisposeAsync() => iniciado ? Container.DisposeAsync().AsTask() : Task.CompletedTask;
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture>
{
    public const string Name = "sql-server";
}
