using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

public class Factory
{
    private const string Topic = "matrix-multiplication-jobs";
    private readonly ILoggerFactory _loggerFactory;

    public Factory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    private static ProducerConfig CreateProducerConfig(string hosts) =>
        new ProducerConfig
        {
            BootstrapServers = hosts,
            AllowAutoCreateTopics = true,
            Acks = Acks.All,
        };

    private static ConsumerConfig CreateConsumerConfig(string hosts) =>
        new ConsumerConfig
        {
            BootstrapServers = hosts,
            GroupId = "matrix-multipliers",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
        };

    public IJobProducer CreateProducer(string hosts)
    {
        var config = CreateProducerConfig(hosts);
        return new JobProducer(_loggerFactory.CreateLogger<JobProducer>(), Topic, config);
    }

    public IJobConsumer CreateConsumer(string hosts)
    {
        var config = CreateConsumerConfig(hosts);
        return new JobConsumer(
            _loggerFactory.CreateLogger<JobConsumer>(),
            _loggerFactory.CreateLogger<JobCompletionNotifier>(),
            Topic,
            config
        );
    }
}
