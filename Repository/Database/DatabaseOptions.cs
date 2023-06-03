namespace Repository.Database;

/// <summary>
/// Database options
/// </summary>
internal class DatabaseOptions
{
    /// <summary>
    /// A name of the database connection string.
    /// </summary>
    public const string ConnectionStringName = "Postgres";

    /// <summary>
    /// Timeout for running the database migration.
    /// </summary>
    public int MigrationTimeoutSeconds { get; set; } = 600;

    /// <summary>
    /// A database connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
