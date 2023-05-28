using System.Linq;
using Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<DatabaseOptions>(o => o.With(builder.Configuration));
builder.Services.AddTransient<DatabaseMigrator>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatabaseMigrator>().Migrate();
}

app.Run();
