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
            EnableAutoCommit = false
        };

    public IProducer<ComputeTaskKey, ComputeTaskValue> CreateComputeTaskProducer(string hosts)
    {
        var config = CreateProducerConfig(hosts);
        return new Producer<ComputeTaskKey, ComputeTaskValue>(
            _loggerFactory.CreateLogger<Producer<ComputeTaskKey, ComputeTaskValue>>(),
            Topic,
            config
        );
    }

    public IConsumer<ComputeTaskKey, ComputeTaskValue> CreateComputeTaskConsumer(string hosts)
    {
        var config = CreateConsumerConfig(hosts);
        return new Consumer<ComputeTaskKey, ComputeTaskValue>(
            _loggerFactory.CreateLogger<Consumer<ComputeTaskKey, ComputeTaskValue>>(),
            _loggerFactory.CreateLogger<MessageCommiter<ComputeTaskKey, ComputeTaskValue>>(),
            Topic,
            config
        );
    }
}
