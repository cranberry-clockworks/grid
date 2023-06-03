namespace Repository.Database;

internal class DatabaseOptions
{
    public const string ConnectionStringName = "Postgres";
    public int MigrationTimeoutSeconds { get; set; } = 600;
    public string ConnectionString { get; set; } = string.Empty;
}
