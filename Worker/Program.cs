using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var kafkaHosts = config.GetValueOrThrow(Configuration.KafkaHosts);

var factory = new Factory(loggerFactory);
using var consumer = factory.CreateComputeTaskConsumer(kafkaHosts);
using var producer = factory.CreateComputedResultProducer(kafkaHosts);

new Processor(loggerFactory.CreateLogger<Processor>(), consumer, producer).Run(
    CancellationToken.None
);
