using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FluentValidation;
using Protocol;
using Scheduler;
using Scheduler.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IJobProducer>(JobProducerFactory.Create);
builder.Services.AddSingleton<IMatrixRepository, MatrixRepository>();
builder.Services.AddSingleton<Distributor>();
builder.Services.AddScoped<MatrixFilesValidator>();
builder.Services.AddScoped<IValidator<IFormFileCollection>, MatrixFilesValidator>();

var app = builder.Build();

app.MapPost(
    "/jobs",
    async (
        [FromServices] Distributor distributor,
        [FromServices] IValidator<IFormFileCollection> validator,
        IFormFileCollection files
    ) =>
    {
        var validationResult = await validator.ValidateAsync(files);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var first = files["First"];
        var second = files["Second"];

        Debug.Assert(first != null);
        Debug.Assert(second != null);

        using var firstStream = first.OpenReadStream();
        using var secondStream = second.OpenReadStream();

        var firstMatrixStream = new MatrixStreamReader(firstStream);
        var secondMatrixStream = new MatrixStreamReader(secondStream);

        if (firstMatrixStream.Columns != secondMatrixStream.Rows)
        {
            return Results.BadRequest("Wrong matrix sizes");
        }

        var taskId = await distributor.ScheduleAsync(firstMatrixStream, secondMatrixStream);
        return Results.Ok(new { taskId });
    }
);

app.MapGet("/jobs/{id}", ([FromRoute] Guid id) => Results.Ok());

app.MapPut(
    "/matricies/{id}",
    (
        [FromServices] Distributor d,
        [FromRoute] Guid id,
        [FromQuery] int row,
        [FromQuery] int column,
        [FromQuery] double value
    ) =>
    {
        Console.WriteLine($"{row} {column} {value}");
        return Results.Ok();
    }
);

app.Run();
