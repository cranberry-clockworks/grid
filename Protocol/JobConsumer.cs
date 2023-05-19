using Confluent.Kafka;
using Microsoft.Extensions.Logging;

internal sealed class JobConsumer : IJobConsumer, IDisposable
{
    private readonly ILogger<JobCompletionNotifier> _notifierLogger;
    private readonly ILogger _consumerLogger;

    private readonly string _topic;
    private readonly IConsumer<Description, Payload> _consumer;

    public JobConsumer(
        ILogger<JobConsumer> consumerLogger,
        ILogger<JobCompletionNotifier> notifierLogger,
        string topic,
        ConsumerConfig config
    )
    {
        _consumerLogger = consumerLogger;
        _notifierLogger = notifierLogger;

        _topic = topic;
        _consumer = new ConsumerBuilder<Description, Payload>(config)
            .SetKeyDeserializer(new ProtobufDeserializer<Description>())
            .SetValueDeserializer(new ProtobufDeserializer<Payload>())
            .Build();

        _consumerLogger.LogInformation("Subscribed to the topic: {Topic}", topic);
    }

    public IEnumerable<Job> ConsumedAsync(CancellationToken token = default)
    {
        _consumer.Subscribe(_topic);
        _consumerLogger.LogInformation("Subscribed to the topic: {Topic}", _topic);

        while (!token.IsCancellationRequested)
        {
            Job job;
            try
            {
                var item = _consumer.Consume(token);
                _consumerLogger.LogInformation(
                    "Consumed an item form the queue. JobId: {JobId}, Row: {Row}, Column: {Column}, RowLength: {RowLength}, ColumnLength: {ColumnLength}",
                    item.Message.Key.JobId,
                    item.Message.Key.Row,
                    item.Message.Key.Column,
                    item.Message.Value.Row.Length,
                    item.Message.Value.Column.Length
                );
                job = new Job(
                    new JobCompletionNotifier(_notifierLogger, _consumer, item),
                    item.Message.Key,
                    item.Message.Value
                );
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
            yield return job;
        }
        _consumer.Close();
        _consumerLogger.LogInformation("Unsubscribed from the topic: {Topic}", _topic);
    }

    public void Dispose()
    {
        _consumer.Dispose();
    }
}
