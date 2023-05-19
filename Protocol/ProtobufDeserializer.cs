using Confluent.Kafka;
using ProtoBuf;

internal class ProtobufDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(
        ReadOnlySpan<byte> data,
        bool isNull,
        Confluent.Kafka.SerializationContext context
    ) => Serializer.Deserialize<T>(data);
}
