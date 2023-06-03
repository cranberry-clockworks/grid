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
using Repository.Database;

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
                Description = "API to create and receive computed matrices",
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
    return new ConsumerProducerFactory(loggerFactory).CreateComputedResultConsumer(options.Value);
});
builder.Services.Configure<DatabaseOptions>(
    options =>
        options.ConnectionString =
            builder.Configuration.GetConnectionString(DatabaseOptions.ConnectionStringName)
            ?? string.Empty
);
builder.Services.AddTransient<DatabaseMigrator>();
builder.Services.AddTransient<IDbConnection>(
    sp =>
        new NpgsqlConnection(
            sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString
        )
);
builder.Services.AddTransient<IMatrixRepository, MatrixRepository>();
builder.Services.AddScoped<IValidator<MatrixCreationOptions>, MatrixCreationOptionsValidator>();
builder.Services.AddHostedService<ComputedResultCollector>();

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
        "/matrices",
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
            return Results.Created($"matrices/{id}", new MatrixCreationResult(id));
        }
    )
    .WithOpenApi()
    .Produces<IDictionary<string, string[]>>(StatusCodes.Status400BadRequest)
    .Produces<MatrixCreationResult>(StatusCodes.Status201Created)
    .WithName("CreateMatrix");

app.MapGet(
        "/matrices/{id}/computed",
        async ([FromServices] IMatrixRepository repository, [FromRoute] int id) =>
            Results.Ok(new ComputationState(id, await repository.IsComputedAsync(id)))
    )
    .WithOpenApi()
    .Produces<ComputationState>()
    .WithName("IsMatrixComputed");

app.MapGet(
        "/matrices/{id:int}",
        async ([FromServices] IMatrixRepository repository, [FromRoute] int id) =>
        {
            var size = await repository.GetMatrixAsync(id);
            if (size == null)
            {
                return Results.NotFound(new { id });
            }

            if (!(await repository.IsComputedAsync(id)))
            {
                return Results.NoContent();
            }

            var values = await repository.GetComputedValuesAsync(id);
            var bytes = MatrixSerializer.Serialize(size.Rows, size.Columns, values);
            return Results.File(
                bytes,
                contentType: "application/octet-stream",
                fileDownloadName: $"{id}"
            );
        }
    )
    .WithName("GetMatrix")
    .Produces<byte[]>()
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

app.Run();
