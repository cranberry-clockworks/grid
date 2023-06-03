using System.Data.Common;
using Protocol;

namespace Repository;

internal class ComputedResultCollector : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IConsumer<ComputedResultKey, ComputedResultValue> _consumer;
    private readonly IMatrixRepository _repository;

    public ComputedResultCollector(
        ILogger<ComputedResultCollector> logger,
        IConsumer<ComputedResultKey, ComputedResultValue> consumer,
        IMatrixRepository repository
    )
    {
        _logger = logger;
        _consumer = consumer;
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var consumable in _consumer.EnumerateConsumableAsync(stoppingToken))
        {
            UpdateValue(consumable);
            consumable.Commit();
        }
    }

    private void UpdateValue(Consumed<ComputedResultKey, ComputedResultValue> consumed)
    {
        try
        {
            _repository.UpdateAsync(
                consumed.Key.MatrixId,
                consumed.Key.Row,
                consumed.Key.Column,
                consumed.Value.Value
            );
        }
        catch (DbException e)
        {
            _logger.LogError(
                e,
                "Failed to update value. Id: {Id}, Row: {Row}, Column: {Column}",
                consumed.Key.MatrixId,
                consumed.Key.Row,
                consumed.Key.Column
            );
        }
    }
}
