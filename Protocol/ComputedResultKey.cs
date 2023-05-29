using ProtoBuf;

namespace Protocol;

[ProtoContract]
public class ComputedResultKey
{
    [ProtoMember(1)]
    public int MatrixId { get; init; }

    [ProtoMember(2)]
    public int Row { get; init; }

    [ProtoMember(3)]
    public int Column { get; init; }
}
