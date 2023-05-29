using Protocol;

namespace Scheduler;

internal static class JobProducerFactory
{
    public static IProducer<ComputeTaskKey, ComputeTaskValue> Create(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var config = services.GetRequiredService<IConfiguration>();
        var hosts = config.GetValueOrThrow(Configuration.KafkaHosts);
        return new Factory(loggerFactory).CreateComputeTaskProducer(hosts);
    }
}
