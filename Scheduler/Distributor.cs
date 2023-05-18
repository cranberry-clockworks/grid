internal class Distributor
{
    private readonly ILogger _logger;

    public Distributor(ILogger<Distributor> logger)
    {
        _logger = logger;
    }

    public Guid ScheduleAsync(IFormFile first, IFormFile second)
    {
        _logger.LogInformation("Scheduled");
        return Guid.NewGuid();
    }
}
