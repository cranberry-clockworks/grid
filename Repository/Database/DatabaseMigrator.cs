using System.Reflection;
using DbUp;
using Microsoft.Extensions.Options;

namespace Repository.Database;

internal class DatabaseMigrator
{
    private readonly ILogger _logger;
    private readonly DatabaseOptions _settings;

    public DatabaseMigrator(ILogger<DatabaseMigrator> logger, IOptions<DatabaseOptions> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    public void Migrate()
    {
        Console.WriteLine(_settings.ConnectionString);
        var engine = DeployChanges.To
            .PostgresqlDatabase(_settings.ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToAutodetectedLog()
            .WithTransactionPerScript()
            .WithExecutionTimeout(TimeSpan.FromSeconds(_settings.MigrationTimeoutSeconds))
            .Build();

        var result = engine.PerformUpgrade();

        if (result.Successful)
        {
            _logger.LogInformation("Migrated successfully");
        }
        else
        {
            _logger.LogError(result.Error, "Failed to migrate the database");
            throw result.Error;
        }
    }
}
