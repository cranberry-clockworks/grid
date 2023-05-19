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

    private static ProducerConfig CreateProducerConfig(IReadOnlyCollection<string> hosts) =>
        new ProducerConfig
        {
            BootstrapServers = string.Join(",", hosts),
            AllowAutoCreateTopics = false,
            Acks = Acks.All,
        };

    private static ConsumerConfig CreateConsumerConfig(IReadOnlyCollection<string> hosts) =>
        new ConsumerConfig
        {
            BootstrapServers = string.Join(",", hosts),
            GroupId = "matrix-multipliers",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

    public IJobProducer CreateProducer(IReadOnlyCollection<string> hosts)
    {
        var config = CreateProducerConfig(hosts);
        return new JobProducer(_loggerFactory.CreateLogger<JobProducer>(), Topic, config);
    }

    public IJobConsumer CreateConsumer(IReadOnlyCollection<string> hosts)
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
