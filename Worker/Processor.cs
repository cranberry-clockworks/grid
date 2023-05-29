using Microsoft.Extensions.Logging;
using Protocol;

internal class Processor
{
    private readonly ILogger _logger;
    private readonly IConsumer<ComputeTaskKey, ComputeTaskValue> _consumer;

    public Processor(
        ILogger<Processor> logger,
        IConsumer<ComputeTaskKey, ComputeTaskValue> consumer
    )
    {
        _logger = logger;
        _consumer = consumer;
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
        var row = consumable.Value.Row;
        var column = consumable.Value.Column;

        double result = 0;
        for (var i = 0; i < row.Length; ++i)
        {
            result += row[i] * column[i];
        }

        _logger.LogInformation(
            "[{Row},{Column}] = {Value}",
            consumable.Value.Row,
            consumable.Value.Column,
            result
        );
    }
}
