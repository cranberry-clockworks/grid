using ProtoBuf;

namespace Protocol;

[ProtoContract]
public class ComputedResultValue
{
    [ProtoMember(1)]
    public double Value { get; init; }
}
