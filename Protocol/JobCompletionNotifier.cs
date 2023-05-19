using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

internal class JobCompletionNotifier
{
    private readonly ILogger<JobCompletionNotifier> _logger;
    private readonly IConsumer<Description, Payload> _consumer;
    private readonly ConsumeResult<Description, Payload> _result;

    public JobCompletionNotifier(
        ILogger<JobCompletionNotifier> logger,
        IConsumer<Description, Payload> consumer,
        ConsumeResult<Description, Payload> result
    )
    {
        _logger = logger;
        _consumer = consumer;
        _result = result;
    }

    public void MarkJobAsComplete()
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
