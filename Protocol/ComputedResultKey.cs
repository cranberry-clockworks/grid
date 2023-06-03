using ProtoBuf;

namespace Protocol;

/// <summary>
/// The key object for the output queue of a worker.
/// </summary>
[ProtoContract]
public class ComputedResultKey
{
    /// <summary>
    /// The id of the matrices product.
    /// </summary>
    [ProtoMember(1)]
    public int MatrixId { get; init; }

    /// <summary>
    /// The row of the computed cell.
    /// </summary>
    [ProtoMember(2)]
    public int Row { get; init; }

    /// <summary>
    /// The column of the computed cell.
    /// </summary>
    [ProtoMember(3)]
    public int Column { get; init; }
}
