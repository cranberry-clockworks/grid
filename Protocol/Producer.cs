using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

internal sealed class Producer<TKey, TValue> : IProducer<TKey, TValue>
{
    private readonly ILogger _logger;
    private string _topic;
    private readonly Confluent.Kafka.IProducer<TKey, TValue> _producer;

    public Producer(ILogger<Producer<TKey, TValue>> logger, string topic, ProducerConfig config)
    {
        _logger = logger;
        _topic = topic;
        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(new ProtobufSerializer<TKey>())
            .SetValueSerializer(new ProtobufSerializer<TValue>())
            .Build();
    }

    public async Task ProduceAsync(TKey key, TValue value)
    {
        var result = await _producer.ProduceAsync(
            _topic,
            new Message<TKey, TValue>() { Key = key, Value = value }
        );
        _logger.LogInformation(
            "Produced item. Topic: {Topic}, Key: {JobId}, Value: {Row}",
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
