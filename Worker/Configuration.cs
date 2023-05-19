using Microsoft.Extensions.Configuration;
using System;

internal static class Configuration
{
    public const string KafkaHosts = "KAFKA_HOSTS";

    public static string GetValueOrThrow(this IConfiguration self, string name)
    {
        return self[name]
            ?? throw new InvalidOperationException(
                $"The environment variable '{name}' is not provided"
            );
    }
}
