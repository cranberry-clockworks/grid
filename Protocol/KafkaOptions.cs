namespace Protocol;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string Hosts { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}
