using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

/// <summary>
/// A class to mark queue items as processed.
/// </summary>
/// <typeparam name="TKey">
/// A key object type of the queue item.
/// </typeparam>
/// <typeparam name="TValue">
/// A value object type of the queue item.
/// </typeparam>
internal class MessageCommitter<TKey, TValue>
{
    private readonly ILogger _logger;
    private readonly Confluent.Kafka.IConsumer<TKey, TValue> _consumer;
    private readonly ConsumeResult<TKey, TValue> _result;

    /// <summary>
    /// Creates the committer.
    /// </summary>
    /// <param name="logger">
    /// A logger instance to write messages.
    /// </param>
    /// <param name="consumer">
    /// A Kafka consumer instance that received the item.
    /// </param>
    /// <param name="result">
    /// The consumed item from the Kafka consumer.
    /// </param>
    public MessageCommitter(
        ILogger<MessageCommitter<TKey, TValue>> logger,
        Confluent.Kafka.IConsumer<TKey, TValue> consumer,
        ConsumeResult<TKey, TValue> result
    )
    {
        _logger = logger;
        _consumer = consumer;
        _result = result;
    }

    public void Commit()
    {
        try
        {
            _consumer.Commit(_result);
        }
        catch (TopicPartitionException e)
        {
            _logger.LogWarning(e, "Failed to commit");
        }
    }
}
