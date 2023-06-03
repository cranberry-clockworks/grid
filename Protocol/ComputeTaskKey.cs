using ProtoBuf;

namespace Protocol;

/// <summary>
/// A key object for the worker task queue.
/// </summary>
[ProtoContract]
public class ComputeTaskKey
{
    /// <summary>
    /// The id of the product matrix.
    /// </summary>
    [ProtoMember(1)]
    public int MatrixId { get; init; }

    /// <summary>
    /// The row of the computed value.
    /// </summary>
    [ProtoMember(2)]
    public int Row { get; init; }

    /// <summary>
    /// The column of the computed value.
    /// </summary>
    [ProtoMember(3)]
    public int Column { get; init; }

    public override string ToString() =>
        $"{{ MatrixId = {MatrixId}, Row = {Row}, Column = {Column}}}";
}
