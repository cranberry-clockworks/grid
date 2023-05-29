using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npgsql;
using Protocol;
using Repository;
using Repository.Models;
using Repository.Validations;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
        c.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "Matrix Repository Api",
                Description = "API to create and receive computed matricies",
                Version = "1.0.0"
            }
        )
);

builder.Services.Configure<KafkaOptions>(
    o => builder.Configuration.GetRequiredSection(KafkaOptions.SectionName).Bind(o)
);
builder.Services.AddSingleton<IConsumer<ComputedResultKey, ComputedResultValue>>(services =>
{
    var options = services.GetRequiredService<IOptions<KafkaOptions>>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    return new Factory(loggerFactory).CreateComputedResultConsumer(options.Value);
});
builder.Services.Configure<DatabaseOptions>(o => o.With(builder.Configuration));
builder.Services.AddTransient<DatabaseMigrator>();
builder.Services.AddTransient<IDbConnection>(
    sp =>
        new NpgsqlConnection(
            sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString
        )
);
builder.Services.AddTransient<IMatrixRepository, MatrixRepository>();
builder.Services.AddScoped<IValidator<MatrixCreationOptions>, MatrixCreationOptionsValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatabaseMigrator>().Migrate();
}

app.MapPost(
        "/matricies",
        async (
            [FromServices] IMatrixRepository repository,
            [FromServices] IValidator<MatrixCreationOptions> validator,
            [FromBody] MatrixCreationOptions options
        ) =>
        {
            var validationResult = await validator.ValidateAsync(options);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var id = await repository.CreateAsync(options.Rows, options.Columns, options.Hash);
            return Results.Created($"matricies/{id}", new MatrixCreationResult(id));
        }
    )
    .WithOpenApi()
    .Produces<IDictionary<string, string[]>>(StatusCodes.Status400BadRequest)
    .Produces<MatrixCreationResult>(StatusCodes.Status201Created)
    .WithName("CreateMatrix");

// TODO
// app.MapGet("/matricies/{id}", () => { });

app.MapGet(
        "/matricices/{id}/computed",
        async ([FromServices] IMatrixRepository repository, [FromRoute] int id) =>
        {
            return Results.Ok(new ComputationState(id, await repository.IsComputedAsync(id)));
        }
    )
    .WithOpenApi()
    .Produces<ComputationState>(StatusCodes.Status200OK)
    .WithName("IsMatrixComputed");

app.Run();
