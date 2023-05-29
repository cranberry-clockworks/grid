using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

public class Factory
{
    private const string ComputeTaskTopic = "matrix-multiplication-tasks";
    private const string ComputedResultTopic = "matrix-multiplication-results";

    private const string ComputeTaskGroupId = "matrix-multipliers";
    private const string ComputedResultGropuId = "matrix-multipliers-result-consumer";

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

    private static ConsumerConfig CreateConsumerConfig(string hosts, string groupId) =>
        new ConsumerConfig
        {
            BootstrapServers = hosts,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
            EnableAutoCommit = false
        };

    public IProducer<ComputeTaskKey, ComputeTaskValue> CreateComputeTaskProducer(
        KafkaOptions options
    )
    {
        var config = CreateProducerConfig(options.Hosts);
        return new Producer<ComputeTaskKey, ComputeTaskValue>(
            _loggerFactory.CreateLogger<Producer<ComputeTaskKey, ComputeTaskValue>>(),
            ComputeTaskTopic,
            config
        );
    }

    public IConsumer<ComputeTaskKey, ComputeTaskValue> CreateComputeTaskConsumer(
        KafkaOptions options
    )
    {
        var config = CreateConsumerConfig(options.Hosts, ComputeTaskGroupId);
        return new Consumer<ComputeTaskKey, ComputeTaskValue>(
            _loggerFactory.CreateLogger<Consumer<ComputeTaskKey, ComputeTaskValue>>(),
            _loggerFactory.CreateLogger<MessageCommiter<ComputeTaskKey, ComputeTaskValue>>(),
            ComputeTaskTopic,
            config
        );
    }

    public IProducer<ComputedResultKey, ComputedResultValue> CreateComputedResultProducer(
        KafkaOptions options
    )
    {
        var config = CreateProducerConfig(options.Hosts);
        return new Producer<ComputedResultKey, ComputedResultValue>(
            _loggerFactory.CreateLogger<Producer<ComputedResultKey, ComputedResultValue>>(),
            ComputedResultTopic,
            config
        );
    }

    public IConsumer<ComputedResultKey, ComputedResultValue> CreateComputedResultConsumer(
        KafkaOptions options
    )
    {
        var config = CreateConsumerConfig(options.Hosts, ComputedResultGropuId);
        return new Consumer<ComputedResultKey, ComputedResultValue>(
            _loggerFactory.CreateLogger<Consumer<ComputedResultKey, ComputedResultValue>>(),
            _loggerFactory.CreateLogger<MessageCommiter<ComputedResultKey, ComputedResultValue>>(),
            ComputedResultTopic,
            config
        );
    }
}
