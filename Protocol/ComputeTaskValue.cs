using ProtoBuf;

namespace Protocol;

[ProtoContract]
public class ComputeTaskValue
{
    [ProtoMember(1)]
    public double[] Row { get; init; } = Array.Empty<double>();

    [ProtoMember(2)]
    public double[] Column { get; init; } = Array.Empty<double>();

    public override string? ToString() =>
        new { RowLength = Row.Length, ColumnLength = Column.Length }.ToString();
}
