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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => Collect(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private void Collect(CancellationToken token)
    {
        foreach (var consumable in _consumer.EnumerateConsumable(token))
        {
            UpdateValue(consumable);
            consumable.Commit();
        }
    }

    private void UpdateValue(Consumable<ComputedResultKey, ComputedResultValue> consumable)
    {
        try
        {
            _repository.Update(
                consumable.Key.MatrixId,
                consumable.Key.Row,
                consumable.Key.Column,
                consumable.Value.Value
            );
        }
        catch (DbException e)
        {
            _logger.LogError(
                e,
                "Failed to update value. Id: {Id}, Row: {Row}, Column: {Column}",
                consumable.Key.MatrixId,
                consumable.Key.Row,
                consumable.Key.Column
            );
        }
    }
}
