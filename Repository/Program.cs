using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using Repository;
using FluentValidation;
using Repository.Validations;

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
builder.Services.AddScoped<IValidator<CreateOptions>, CreateOptionsValidator>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatabaseMigrator>().Migrate();
}

app.MapPost(
    "/matricies",
    async (
        [FromServices] IMatrixRepository repository,
        [FromServices] IValidator<CreateOptions> validator,
        [FromBody] CreateOptions options
    ) =>
    {
        var validationResult = await validator.ValidateAsync(options);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var id = await repository.CreateAsync(options.Rows, options.Columns, options.Hash);
        return Results.Created($"matricies/{id}", id);
    }
);

app.Run();
