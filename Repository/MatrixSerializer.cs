using Repository.Database;
using System.Buffers;

namespace Repository;

internal static class MatrixSerializer
{
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
