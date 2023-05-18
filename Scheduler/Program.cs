using FluentValidation;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddScoped<MatrixFilesValidator>();
builder.Services.AddSingleton<Distributor>();
builder.Services.AddScoped<IValidator<IFormFileCollection>, MatrixFilesValidator>();

var app = builder.Build();

app.MapPost(
    "/multiply",
    async (Distributor d, IValidator<IFormFileCollection> validator, IFormFileCollection files)
    =>
    {
        var validationResult = await validator.ValidateAsync(files);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        d.ScheduleAsync();
        return Results.Ok();
    });

app.Run();
