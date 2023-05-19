namespace Protocol;

public interface IJobProducer : IDisposable
{
    Task PublishAsync(Description description, Payload job);
}
