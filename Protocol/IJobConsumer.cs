namespace Protocol;

public interface IJobConsumer : IDisposable
{
    public IEnumerable<Job> ConsumedJobs(CancellationToken token);
}
