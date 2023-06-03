namespace Protocol;

/// <summary>
/// Enqueues items to a queue.
/// </summary>
/// <typeparam name="TKey">
/// The key object type of an enqueued item.
/// </typeparam>
/// <typeparam name="TValue">
/// The value object type of an enqueued item.
/// </typeparam>
public interface IProducer<in TKey, in TValue> : IDisposable
{
    /// <summary>
    /// Enqueues item into the queue.
    /// </summary>
    /// <param name="key">
    /// The key of the item.
    /// </param>
    /// <param name="value">
    /// The value of the item.
    /// </param>
    /// <param name="token">
    /// A token to cancel the operation.
    /// </param>
    Task ProduceAsync(TKey key, TValue value, CancellationToken token);

    /// <summary>
    /// Enqueues item into the queue.
    /// </summary>
    /// <param name="key">
    /// The key of the item.
    /// </param>
    /// <param name="value">
    /// The value of the item.
    /// </param>
    void Produce(TKey key, TValue value);
}
