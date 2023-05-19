using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var kafkaHosts = config.GetValueOrThrow(Configuration.KafkaHosts);

logger.LogInformation("Running worker");

using var producer = new Factory(loggerFactory).CreateProducer(kafkaHosts);
await producer.PublishAsync(
    new Description()
    {
        JobId = Guid.NewGuid(),
        Row = 0,
        Column = 0,
    },
    new Payload() { Row = new double[] { 1, 2, 3 }, Column = new double[] { 4, 5, 6 } }
);

using var consumer = new Factory(loggerFactory).CreateConsumer(kafkaHosts);

new Processor(loggerFactory.CreateLogger<Processor>(), consumer).Run(CancellationToken.None);
