using FluentValidation;
using Microsoft.Extensions.Options;
using Protocol;
using Scheduler;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Logging.AddConsole();

builder.Services.Configure<KafkaOptions>(
    o => builder.Configuration.GetRequiredSection(KafkaOptions.SectionName).Bind(o)
);
builder.Services.AddSingleton<IProducer<ComputeTaskKey, ComputeTaskValue>>(services =>
{
    var options = services.GetRequiredService<IOptions<KafkaOptions>>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    return new Factory(loggerFactory).CreateComputeTaskProducer(options.Value);
});
builder.Services.Configure<MatrixRepositoryOptions>(o => o.With(builder.Configuration));
builder.Services
    .AddHttpClient<IMatrixRepository, MatrixRepository>(
        (services, client) =>
            client.BaseAddress = new Uri(
                services.GetRequiredService<IOptions<MatrixRepositoryOptions>>().Value.Host
            )
    )
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler((provider, _) => MatrixRepository.GetRetryPolicy(provider));
builder.Services.AddSingleton<IMatrixRepository, MatrixRepository>();

builder.Services.AddSingleton<Distributor>();
builder.Services.AddScoped<IValidator<IFormFileCollection>, MatrixFilesValidator>();

var app = builder.Build();

app.MapPost("/jobs", JobController.ScheduleAsync);

app.Run();
