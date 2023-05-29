using ProtoBuf;

namespace Protocol;

[ProtoContract]
public class ComputeTaskKey
{
    [ProtoMember(1)]
    public Guid JobId { get; init; }

    [ProtoMember(2)]
    public int Row { get; init; }

    [ProtoMember(3)]
    public int Column { get; init; }
}
