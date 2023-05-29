using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

internal sealed class Consumer<TKey, TValue> : IConsumer<TKey, TValue>, IDisposable
{
    private readonly ILogger<MessageCommiter<TKey, TValue>> _notifierLogger;
    private readonly ILogger _consumerLogger;

    private readonly string _topic;
    private readonly Confluent.Kafka.IConsumer<TKey, TValue> _consumer;

    public Consumer(
        ILogger<Consumer<TKey, TValue>> consumerLogger,
        ILogger<MessageCommiter<TKey, TValue>> notifierLogger,
        string topic,
        ConsumerConfig config
    )
    {
        _consumerLogger = consumerLogger;
        _notifierLogger = notifierLogger;

        _topic = topic;
        _consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(new ProtobufDeserializer<TKey>())
            .SetValueDeserializer(new ProtobufDeserializer<TValue>())
            .Build();

        _consumerLogger.LogInformation("Subscribed to the topic: {Topic}", topic);
    }

    public IEnumerable<Consumable<TKey, TValue>> EnumerateConsumable(
        CancellationToken token = default
    )
    {
        _consumer.Subscribe(_topic);
        _consumerLogger.LogInformation("Subscribed to the topic: {Topic}", _topic);

        while (!token.IsCancellationRequested)
        {
            Consumable<TKey, TValue> consumable;
            try
            {
                consumable = Consume(token);
            }
            catch (ConsumeException e)
            {
                _consumerLogger.LogError(e, "Failed to consume the item form the queue");
                continue;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            yield return consumable;
        }

        _consumer.Close();
        _consumerLogger.LogInformation("Unsubscribed from the topic: {Topic}", _topic);
    }

    private Consumable<TKey, TValue> Consume(CancellationToken token)
    {
        Consumable<TKey, TValue> consumable;
        var item = _consumer.Consume(token);
        _consumerLogger.LogInformation(
            "Fetched item. Topic: {Topic} Key: {Key}, Value: {Value}",
            _topic,
            item.Message.Key,
            item.Message.Value
        );
        consumable = new Consumable<TKey, TValue>(
            new MessageCommiter<TKey, TValue>(_notifierLogger, _consumer, item),
            item.Message.Key,
            item.Message.Value
        );
        return consumable;
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}
