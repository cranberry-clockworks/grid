using FluentValidation;
using Microsoft.Extensions.Options;
using Protocol;
using Scheduler.Controller;
using Scheduler.Repository;
using Scheduler.Scheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<KafkaOptions>(
    options => builder.Configuration.GetRequiredSection(KafkaOptions.SectionName).Bind(options)
);
builder.Services.Configure<MatrixRepositoryOptions>(
    options =>
        builder.Configuration.GetRequiredSection(MatrixRepositoryOptions.SectionName).Bind(options)
);

builder.Services.AddSingleton<IProducer<ComputeTaskKey, ComputeTaskValue>>(services =>
{
    var options = services.GetRequiredService<IOptions<KafkaOptions>>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    return new ConsumerProducerFactory(loggerFactory).CreateComputeTaskProducer(options.Value);
});

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

builder.Services.AddSingleton<ProductTaskScheduler>();
builder.Services.AddScoped<IValidator<IFormFileCollection>, MatrixFilesValidator>();

var app = builder.Build();

app.MapPost("/matrices", MatrixController.ScheduleAsync);

app.Run();
