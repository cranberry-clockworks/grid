using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

internal class MessageCommiter<TKey, TValue>
{
    private readonly ILogger _logger;
    private readonly Confluent.Kafka.IConsumer<TKey, TValue> _consumer;
    private readonly ConsumeResult<TKey, TValue> _result;

    public MessageCommiter(
        ILogger<MessageCommiter<TKey, TValue>> logger,
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
