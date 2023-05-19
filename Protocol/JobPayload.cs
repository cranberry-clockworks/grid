using ProtoBuf;

[ProtoContract]
public class Payload
{
    [ProtoMember(1)]
    public double[] Row { get; init; } = Array.Empty<double>();

    [ProtoMember(2)]
    public double[] Column { get; init; } = Array.Empty<double>();
}
