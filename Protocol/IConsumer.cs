namespace Protocol;

/// <summary>
/// Consumer for messages from a queue.
/// </summary>
/// <typeparam name="TKey">
/// The key type of queue items.
/// </typeparam>
/// <typeparam name="TValue">
/// The value type of queue items.
/// </typeparam>
public interface IConsumer<TKey, TValue> : IDisposable
{
    /// <summary>
    /// Waits and gets new items in the queue.
    /// </summary>
    /// <param name="token">
    /// A token to cancel operation.
    /// </param>
    /// <returns>
    /// Consumed items from the queue.
    /// </returns>
    /// <remarks>
    /// Should be treated as long running task and placed in a dedicated thread if possible.
    /// </remarks>
    IAsyncEnumerable<Consumed<TKey, TValue>> EnumerateConsumableAsync(CancellationToken token);
}
