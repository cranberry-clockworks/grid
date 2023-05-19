public class Job
{
    private readonly JobCompletionNotifier _notifier;

    internal Job(JobCompletionNotifier notifier, Description description, Payload job)
    {
        _notifier = notifier;
    }

    void MarkAsCompleted()
    {
        _notifier.MarkJobAsComplete();
    }
}
