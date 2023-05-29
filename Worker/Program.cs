using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var kafkaHosts = config.GetValueOrThrow(Configuration.KafkaHosts);

using var consumer = new Factory(loggerFactory).CreateComputeTaskConsumer(kafkaHosts);

new Processor(loggerFactory.CreateLogger<Processor>(), consumer).Run(CancellationToken.None);
