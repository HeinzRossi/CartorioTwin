namespace CartorioTwin.Infrastructure.Tests;

internal sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.Equals(
                Environment.GetEnvironmentVariable("CARTORIOTWIN_SKIP_SQLSERVER_TESTS"),
                "true",
                StringComparison.OrdinalIgnoreCase))
        {
            Skip = "Executado no job persistence-integration com Docker Linux.";
        }
    }
}
