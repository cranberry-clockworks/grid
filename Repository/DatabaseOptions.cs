namespace Repository;

internal class DatabaseOptions
{
    public const string ConnectionStringName = "Postgres";
    public int MigrationTimeoutSeconds { get; set; } = 600;
    public string ConnectionString { get; set; } = string.Empty;

    public void With(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString(ConnectionStringName) ?? string.Empty;
    }
}
