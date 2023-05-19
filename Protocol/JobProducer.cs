using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Protocol;

internal sealed class JobProducer : IJobProducer
{
    private readonly ILogger<JobProducer> _logger;
    private string _topic;
    private readonly IProducer<Description, Payload> _producer;

    public JobProducer(ILogger<JobProducer> logger, string topic, ProducerConfig config)
    {
        _logger = logger;
        _topic = topic;
        _producer = new ProducerBuilder<Description, Payload>(config)
            .SetKeySerializer(new ProtobufSerializer<Description>())
            .SetValueSerializer(new ProtobufSerializer<Payload>())
            .Build();
    }

    public async Task PublishAsync(Description description, Payload payload)
    {
        var result = await _producer.ProduceAsync(
            _topic,
            new Message<Description, Payload>() { Key = description, Value = payload }
        );
        _logger.LogInformation(
            "Produced job. Topic: {Topic}, JobId: {JobId}, Row: {Row} Column: {Column}, RowLength: {RowLength}, ColumnLength: {ColumnLength}",
            _topic,
            description.JobId,
            description.Row,
            description.Column,
            payload.Row.Length,
            payload.Column.Length
        );
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
