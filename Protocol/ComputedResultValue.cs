using System.Globalization;
using ProtoBuf;

namespace Protocol;

/// <summary>
/// The value object of the output queue of a worker.
/// </summary>
[ProtoContract]
public class ComputedResultValue
{
    /// <summary>
    /// Computed value of the single cell of a matrix product.
    /// </summary>
    [ProtoMember(1)]
    public double Value { get; init; }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
