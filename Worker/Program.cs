using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Protocol;
using Worker;

var options = new Options();
var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
config.GetRequiredSection(Options.SectionName).Bind(options);

var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(config).AddConsole());

var factory = new ConsumerProducerFactory(loggerFactory);
using var consumer = factory.CreateComputeTaskConsumer(options.Input);
using var producer = factory.CreateComputedResultProducer(options.Output);

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

new Processor(loggerFactory.CreateLogger<Processor>(), consumer, producer).Run(cts.Token);
