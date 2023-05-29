namespace Protocol;

public record Consumable<TKey, TValue>
{
    public TKey Key { get; }
    public TValue Value { get; }

    private readonly MessageCommiter<TKey, TValue> _notifier;

    internal Consumable(MessageCommiter<TKey, TValue> notifier, TKey key, TValue value)
    {
        _notifier = notifier;
        Key = key;
        Value = value;
    }

    public void Commit()
    {
        _notifier.Commit();
    }
}
