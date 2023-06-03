using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

/// <summary>
/// Kafka consumer.
/// </summary>
/// <typeparam name="TKey">
/// The key type of the consumed item.
/// </typeparam>
/// <typeparam name="TValue">
/// The value type of the consumed item.
/// </typeparam>
/// <remarks>
/// Implementation made with "async over sync" pattern.
/// </remarks>
internal sealed class Consumer<TKey, TValue> : IConsumer<TKey, TValue>
{
    private readonly ILogger<MessageCommitter<TKey, TValue>> _notifierLogger;
    private readonly ILogger _consumerLogger;

    private readonly string _topic;
    private readonly Confluent.Kafka.IConsumer<TKey, TValue> _consumer;

    private readonly TimeSpan _recoveryDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Creates the consumer.
    /// </summary>
    /// <param name="consumerLogger">
    /// A logger instance for the consumer itself.
    /// </param>
    /// <param name="notifierLogger">
    /// A logger instance for the message committer helper classes.
    /// </param>
    /// <param name="topic">
    /// The Kafka topic to subscribe
    /// </param>
    /// <param name="config">
    /// The configuration for the consumer.
    /// </param>
    public Consumer(
        ILogger<Consumer<TKey, TValue>> consumerLogger,
        ILogger<MessageCommitter<TKey, TValue>> notifierLogger,
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

    /// <inheritdoc />
    public async IAsyncEnumerable<Consumed<TKey, TValue>> EnumerateConsumableAsync(
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                _consumer.Subscribe(_topic);
                _consumerLogger.LogInformation("Subscribed to the topic: {Topic}", _topic);
            }
            catch (Exception e) when (e is KafkaException or SocketException)
            {
                _consumerLogger.LogError(
                    e,
                    "Failed to subscribe on the topic: {Topic}. Waiting {Delay} seconds before next try.",
                    _topic,
                    _recoveryDelay.Seconds
                );
                await Task.Delay(_recoveryDelay, token);
            }
        }

        foreach (var consumed in ConsumeMany(token))
        {
            yield return consumed;
        }

        _consumer.Close();
        _consumerLogger.LogInformation("Unsubscribed from the topic: {Topic}", _topic);
    }

    private IEnumerable<Consumed<TKey, TValue>> ConsumeMany(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Consumed<TKey, TValue> consumed;
            try
            {
                consumed = ConsumeSingle(token);
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

            yield return consumed;
        }
    }

    private Consumed<TKey, TValue> ConsumeSingle(CancellationToken token)
    {
        var item = _consumer.Consume(token);
        _consumerLogger.LogDebug(
            "Fetched item. Topic: {Topic} Key: {Key}, Value: {Value}",
            _topic,
            item.Message.Key,
            item.Message.Value
        );
        var consumed = new Consumed<TKey, TValue>(
            new MessageCommitter<TKey, TValue>(_notifierLogger, _consumer, item),
            item.Message.Key,
            item.Message.Value
        );
        return consumed;
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}
