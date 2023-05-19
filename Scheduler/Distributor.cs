using Protocol;

internal class Distributor
{
    private readonly ILogger _logger;
    private readonly IJobProducer _producer;

    public Distributor(ILogger<Distributor> logger, IJobProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }

    public Guid ScheduleAsync(IFormFile first, IFormFile second)
    {
        _logger.LogInformation("Scheduled");
        return Guid.NewGuid();
    }
}
