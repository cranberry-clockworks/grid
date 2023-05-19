using Microsoft.Extensions.Logging;
using Protocol;

internal class Processor
{
    private readonly ILogger _logger;
    private readonly IJobConsumer _consumer;

    public Processor(ILogger<Processor> logger, IJobConsumer consumer)
    {
        _logger = logger;
        _consumer = consumer;
    }

    public void Run(CancellationToken token)
    {
        foreach (var job in _consumer.ConsumedJobs(token))
        {
            if (IsValid(job))
            {
                Process(job);
                continue;
            }

            _logger.LogError(
                "Dimensions of row doesn't equal the column. Row: {RowLength}, Column: {ColumnLength}",
                job.Payload.Row.Length,
                job.Payload.Column.Length
            );
            job.MarkAsCompleted();
            continue;
        }
    }

    private bool IsValid(Job job) => job.Payload.Row.Length == job.Payload.Column.Length;

    private void Process(Job job)
    {
        var row = job.Payload.Row;
        var column = job.Payload.Column;

        double result = 0;
        for (var i = 0; i < row.Length; ++i)
        {
            result += row[i] * column[i];
        }

        _logger.LogInformation(
            "[{Row},{Column}] = {Value}",
            job.Description.Row,
            job.Description.Column,
            result
        );
    }
}
