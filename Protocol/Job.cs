namespace Protocol;

public class Job
{
    private readonly JobCompletionNotifier _notifier;

    public Description Description { get; }
    public Payload Payload { get; }

    internal Job(JobCompletionNotifier notifier, Description description, Payload payload)
    {
        _notifier = notifier;
        Description = description;
        Payload = payload;
    }

    public void MarkAsCompleted()
    {
        _notifier.MarkJobAsComplete();
    }
}
