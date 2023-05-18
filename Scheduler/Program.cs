using System.Diagnostics;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddScoped<MatrixFilesValidator>();
builder.Services.AddSingleton<Distributor>();
builder.Services.AddScoped<IValidator<IFormFileCollection>, MatrixFilesValidator>();

var app = builder.Build();

app.MapPost(
    "/multiply",
    async (
        Distributor distributor,
        IValidator<IFormFileCollection> validator,
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

        var taskId = distributor.ScheduleAsync(first, second);
        return Results.Ok(new { taskId });
    }
);

app.Run();
