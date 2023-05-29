namespace Protocol;

public interface IConsumer<TKey, TValue> : IDisposable
{
    IEnumerable<Consumable<TKey, TValue>> EnumerateConsumable(CancellationToken token);
}
