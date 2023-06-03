using ProtoBuf;

namespace Protocol;

/// <summary>
/// The value object for the worker task queue.
/// </summary>
[ProtoContract]
public class ComputeTaskValue
{
    /// <summary>
    /// The row to compute the matrix product.
    /// </summary>
    /// <remarks>
    /// Must have same length as <see cref="Column"/>
    /// </remarks>
    [ProtoMember(1)]
    public double[] Row { get; init; } = Array.Empty<double>();

    /// <summary>
    /// The column to compute the matrix product.
    /// </summary>
    [ProtoMember(2)]
    public double[] Column { get; init; } = Array.Empty<double>();
}
