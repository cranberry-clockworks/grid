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

    public void Run(CancellationToken token)
    {
        foreach (var consumable in _consumer.EnumerateConsumable(token))
        {
            if (IsValid(consumable.Value))
            {
                Process(consumable);
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

    private void Process(Consumable<ComputeTaskKey, ComputeTaskValue> consumable)
    {
        using var logger = _logger.BeginScope(
            "Processing task. Id: {Id}, Row: {Row}, Column: {Column}",
            consumable.Key.MatrixId,
            consumable.Key.Row,
            consumable.Key.Column
        );

        var result = Compute(consumable.Value);

        _logger.LogTrace("Computed result: {Result}", result);

        _producer.Produce(
            new ComputedResultKey
            {
                MatrixId = consumable.Key.MatrixId,
                Row = consumable.Key.Row,
                Column = consumable.Key.Column
            },
            new ComputedResultValue { Value = result }
        );
    }

    private double Compute(ComputeTaskValue task)
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

    private void PushFurther(ComputeTaskKey task, double value) { }
}
