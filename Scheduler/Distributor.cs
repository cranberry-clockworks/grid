internal class Distributor
{
    private readonly ILogger _logger;
    public Distributor(ILogger<Distributor> logger)
    {
        _logger = logger;
    }

    public void ScheduleAsync()
    {
        _logger.LogInformation("Scheduled");
    }
}