namespace Scheduler;

internal interface IMatrixRepository
{
    Task<int> CreateAsync(int rows, int columns, string hash, CancellationToken token);
    Task<bool> IsComputed(int id, CancellationToken token);
}
