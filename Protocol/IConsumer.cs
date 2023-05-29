namespace Protocol;

public interface IConsumer<TKey, TValue> : IDisposable
{
    public IEnumerable<Consumable<TKey, TValue>> EnumerateConsumable(CancellationToken token);
}
