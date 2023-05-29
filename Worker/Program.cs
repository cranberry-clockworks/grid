using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var options = new KafkaOptions();
config.GetRequiredSection(KafkaOptions.SectionName).Bind(options);

var factory = new Factory(loggerFactory);
using var consumer = factory.CreateComputeTaskConsumer(options);
using var producer = factory.CreateComputedResultProducer(options);

new Processor(loggerFactory.CreateLogger<Processor>(), consumer, producer).Run(
    CancellationToken.None
);
