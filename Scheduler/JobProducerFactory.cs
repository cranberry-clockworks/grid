using Protocol;

namespace Scheduler;

internal static class JobProducerFactory
{
    public static IJobProducer Create(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var config = services.GetRequiredService<IConfiguration>();
        var hosts = config.GetValueOrThrow(Configuration.KafkaHosts);
        return new Factory(loggerFactory).CreateProducer(hosts);
    }
}
