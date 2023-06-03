using Microsoft.Extensions.Logging;
using Protocol;

namespace Worker;

internal class Processor
{
    private readonly ILogger _logger;
    private readonly IConsumer<ComputeTaskKey, ComputeTaskValue> _consumer;
    private readonly IProducer<ComputedResultKey, ComputedResultValue> _producer;

    public Processor(
        ILogger<Processor> logger,
        IConsumer<ComputeTaskKey, ComputeTaskValue> consumer,
        IProducer<ComputedResultKey, ComputedResultValue> producer
    )
    {
        _logger = logger;
        _consumer = consumer;
        _producer = producer;
    }

    public async Task RunAsync(CancellationToken token)
    {
        await foreach (var consumable in _consumer.EnumerateConsumableAsync(token))
        {
            if (IsValid(consumable.Value))
            {
                await ProcessAsync(consumable, token);
            }
            else
            {
                _logger.LogError(
                    "Dimensions of row doesn't equal the column. Row: {RowLength}, Column: {ColumnLength}",
                    consumable.Value.Row.Length,
                    consumable.Value.Column.Length
                );
            }

            consumable.Commit();
        }
    }

    private bool IsValid(ComputeTaskValue value) => value.Row.Length == value.Column.Length;

    private async Task ProcessAsync(
        Consumed<ComputeTaskKey, ComputeTaskValue> consumed,
        CancellationToken token
    )
    {
        using var logger = _logger.BeginScope(
            "Processing task. Id: {Id}, Row: {Row}, Column: {Column}",
            consumed.Key.MatrixId,
            consumed.Key.Row,
            consumed.Key.Column
        );

        var result = Compute(consumed.Value);

        _logger.LogTrace("Computed result: {Result}", result);

        await _producer.ProduceAsync(
            new ComputedResultKey
            {
                MatrixId = consumed.Key.MatrixId,
                Row = consumed.Key.Row,
                Column = consumed.Key.Column
            },
            new ComputedResultValue { Value = result },
            token
        );
    }

    private static double Compute(ComputeTaskValue task)
    {
        var row = task.Row;
        var column = task.Column;

        double result = 0;
        for (var i = 0; i < row.Length; ++i)
        {
            result += row[i] * column[i];
        }

        return result;
    }
}
