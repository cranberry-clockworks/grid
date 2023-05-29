using ProtoBuf;

namespace Protocol;

[ProtoContract]
public class ComputedResultValue
{
    [ProtoMember(1)]
    public double Value { get; init; }

    public override string? ToString() => new { Value }.ToString();
}
