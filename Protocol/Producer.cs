using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

/// <summary>
/// Kafka producer.
/// </summary>
/// <typeparam name="TKey">
/// The key object type of the produced items.
/// </typeparam>
/// <typeparam name="TValue">
/// The value object type of the produced items.
/// </typeparam>
internal sealed class Producer<TKey, TValue> : IProducer<TKey, TValue>
{
    private readonly ILogger _logger;
    private readonly string _topic;
    private readonly Confluent.Kafka.IProducer<TKey, TValue> _producer;

    /// <summary>
    /// Creates Kafka producer.
    /// </summary>
    /// <param name="logger">
    /// A logger instance to write messages.
    /// </param>
    /// <param name="topic">
    /// A topic to which items are producer.
    /// </param>
    /// <param name="config">
    /// The configuration of the producer.
    /// </param>
    public Producer(ILogger<Producer<TKey, TValue>> logger, string topic, ProducerConfig config)
    {
        _logger = logger;
        _topic = topic;
        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(new ProtobufSerializer<TKey>())
            .SetValueSerializer(new ProtobufSerializer<TValue>())
            .Build();
    }

    /// <inheritdoc />
    public async Task ProduceAsync(TKey key, TValue value, CancellationToken token)
    {
        await _producer.ProduceAsync(
            _topic,
            new Message<TKey, TValue>() { Key = key, Value = value },
            token
        );

        _logger.LogInformation(
            "Produced item. Topic: {Topic}, Key: {Key}, Value: {Value}",
            _topic,
            key,
            value
        );
    }

    /// <inheritdoc />
    public void Produce(TKey key, TValue value)
    {
        _producer.Produce(_topic, new Message<TKey, TValue>() { Key = key, Value = value });

        _logger.LogInformation(
            "Produced item. Topic: {Topic}, Key: {Key}, Value: {Value}",
            _topic,
            key,
            value
        );
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
