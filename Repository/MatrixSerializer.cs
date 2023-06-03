using Repository.Database;
using System.Buffers;

namespace Repository;

/// <summary>
/// A class to serialize a matrix.
/// </summary>
internal static class MatrixSerializer
{
    /// <summary>
    /// Serializes matrix to stream of bytes.
    /// </summary>
    /// <param name="rows">
    /// The rows count of the serialized matrix.
    /// </param>
    /// <param name="columns">
    /// The columns count of the serialized matrix.
    /// </param>
    /// <param name="orderedValues">
    /// The cell values of the matrix sorted by rows than by columns.
    /// </param>
    /// <returns>
    /// Serialized matrix.
    /// </returns>
    public static byte[] Serialize(int rows, int columns, IEnumerable<double> orderedValues)
    {
        var length = sizeof(int) * 2 + sizeof(double) * rows * columns;

        var buffer = new byte[length];
        using var stream = new MemoryStream(buffer);
        using var writer = new BinaryWriter(stream);

        writer.Write(rows);
        writer.Write(columns);

        foreach (var value in orderedValues)
        {
            writer.Write(value);
        }

        return buffer;
    }
}
