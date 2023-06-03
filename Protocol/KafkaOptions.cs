namespace Protocol;

/// <summary>
/// Options for Kafka producers or consumers.
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// A name of the configuration section.
    /// </summary>
    public const string SectionName = "Kafka";

    /// <summary>
    /// A hosts list of Kafka brokers.
    /// </summary>
    public string Hosts { get; set; } = string.Empty;

    /// <summary>
    /// A used Kafka topic to push or fetch messages.
    /// </summary>
    public string Topic { get; set; } = string.Empty;
}
