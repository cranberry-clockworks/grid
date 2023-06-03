using System.Data.Common;
using Protocol;

namespace Repository;

/// <summary>
/// A computed result collector from workers.
/// </summary>
internal class ComputedResultCollector : IHostedService, IDisposable
{
    private readonly ILogger _logger;
    private readonly IConsumer<ComputedResultKey, ComputedResultValue> _consumer;
    private readonly IMatrixRepository _repository;

    private Task? _task;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Creates the collector.
    /// </summary>
    /// <param name="logger">
    /// A logger instance to write the messages.
    /// </param>
    /// <param name="consumer">
    /// A consumer of computed results from the worker output queue.
    /// </param>
    /// <param name="repository">
    /// The matrix repository to store results.
    /// </param>
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

    private void Collect(CancellationToken token)
    {
        foreach (var consumable in _consumer.EnumerateConsumable(token))
        {
            UpdateValue(consumable);
            consumable.Commit();
        }
    }

    private void UpdateValue(Consumed<ComputedResultKey, ComputedResultValue> consumed)
    {
        try
        {
            _repository.Update(
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_task != null)
            return Task.CompletedTask;

        _task = Task.Run(() => Collect(_cancellationTokenSource.Token), CancellationToken.None);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_task == null)
            return;

        if (!_task.IsCompleted)
        {
            _cancellationTokenSource.Cancel();
            await _task;
        }

        _task = null;
        if (_cancellationTokenSource.TryReset())
        {
            throw new InvalidOperationException("Failed to reset cancellation token source");
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}
