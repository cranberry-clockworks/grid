using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;
using Worker;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var options = new Options();
config.GetRequiredSection(Options.SectionName).Bind(options);

var factory = new Factory(loggerFactory);
using var consumer = factory.CreateComputeTaskConsumer(options.Input);
using var producer = factory.CreateComputedResultProducer(options.Output);

new Processor(loggerFactory.CreateLogger<Processor>(), consumer, producer).Run(
    CancellationToken.None
);
