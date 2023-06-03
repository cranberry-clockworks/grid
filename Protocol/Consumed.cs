namespace Protocol;

/// <summary>
/// A consumed item from a queue.
/// </summary>
/// <typeparam name="TKey">
/// The type of the key object in the queue.
/// </typeparam>
/// <typeparam name="TValue">
/// The type of the value object in the queue.
/// </typeparam>
public record Consumed<TKey, TValue>
{
    /// <summary>
    /// The key object.
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// The value object.
    /// </summary>
    public TValue Value { get; }

    private readonly MessageCommitter<TKey, TValue> _committer;

    /// <summary>
    /// Creates the consumed object.
    /// </summary>
    /// <param name="committer">
    /// The queue item committer.
    /// </param>
    /// <param name="key">
    /// The key of the consumed item.
    /// </param>
    /// <param name="value">
    /// The value of the consumed item.
    /// </param>
    internal Consumed(MessageCommitter<TKey, TValue> committer, TKey key, TValue value)
    {
        _committer = committer;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Marks the consumed item as processed.
    /// </summary>
    public void Commit()
    {
        _committer.Commit();
    }
}
