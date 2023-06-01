using Protocol;

namespace Worker;

internal class Options
{
    public const string SectionName = "Kafka";
    public KafkaOptions Input { get; init; } = new KafkaOptions();
    public KafkaOptions Output { get; init; } = new KafkaOptions();
}
