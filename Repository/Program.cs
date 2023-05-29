using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<DatabaseOptions>(o => o.With(builder.Configuration));
builder.Services.AddTransient<DatabaseMigrator>();
builder.Services.AddTransient<IDbConnection>(
    sp =>
        new NpgsqlConnection(
            sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString
        )
);
builder.Services.AddTransient<IMatrixRepository, MatrixRepository>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatabaseMigrator>().Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var r = scope.ServiceProvider.GetRequiredService<IMatrixRepository>();
    var id = r.Create(5, 5, "FOO");
    r.Update(id, 0, 0, 10);
    r.Update(id, 0, 0, 20);
    r.Update(id, 0, 7, 10);
}

app.Run();
