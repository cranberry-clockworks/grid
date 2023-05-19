using Confluent.Kafka;
using ProtoBuf;

namespace Protocol;

internal class ProtobufSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, Confluent.Kafka.SerializationContext context)
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, data);
        return stream.ToArray();
    }
}
