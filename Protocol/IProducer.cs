namespace Protocol;

public interface IProducer<TKey, TValue> : IDisposable
{
    Task ProduceAsync(TKey key, TValue value, CancellationToken token);
}
