using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

public class ConsumerProducerFactory
{
    private const string ComputeTaskGroupId = "multipliers";
    private const string ComputedResultGroupId = "collectors";

    private readonly ILoggerFactory _loggerFactory;

    public ConsumerProducerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    private static ProducerConfig CreateProducerConfig(string hosts) =>
        new()
        {
            BootstrapServers = hosts,
            AllowAutoCreateTopics = false,
            Acks = Acks.All
        };

    private static ConsumerConfig CreateConsumerConfig(string hosts, string groupId) =>
        new()
        {
            BootstrapServers = hosts,
            GroupId = groupId,
            EnableAutoCommit = false,
        };

    public IProducer<ComputeTaskKey, ComputeTaskValue> CreateComputeTaskProducer(
        KafkaOptions options
    )
    {
        var config = CreateProducerConfig(options.Hosts);
        return new Producer<ComputeTaskKey, ComputeTaskValue>(
            _loggerFactory.CreateLogger<Producer<ComputeTaskKey, ComputeTaskValue>>(),
            options.Topic,
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
            _loggerFactory.CreateLogger<MessageCommitter<ComputeTaskKey, ComputeTaskValue>>(),
            options.Topic,
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
            options.Topic,
            config
        );
    }

    public IConsumer<ComputedResultKey, ComputedResultValue> CreateComputedResultConsumer(
        KafkaOptions options
    )
    {
        var config = CreateConsumerConfig(options.Hosts, ComputedResultGroupId);
        return new Consumer<ComputedResultKey, ComputedResultValue>(
            _loggerFactory.CreateLogger<Consumer<ComputedResultKey, ComputedResultValue>>(),
            _loggerFactory.CreateLogger<MessageCommitter<ComputedResultKey, ComputedResultValue>>(),
            options.Topic,
            config
        );
    }
}
