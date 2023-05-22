using System.Text;

namespace Scheduler;

internal class MatrixStreamReader
{
    public int Rows { get; }
    public int Columns { get; }

    private readonly Stream _stream;
    private readonly BinaryReader _reader;

    private long _start;

    public MatrixStreamReader(Stream stream)
    {
        var reader = new BinaryReader(stream, Encoding.ASCII, true);
        _stream = stream;
        _reader = reader;

        Rows = reader.ReadInt32();
        Columns = reader.ReadInt32();

        _start = stream.Position;
    }

    public double[] ReadRow() => Read(Columns);

    public double[] ReadColumn() => Read(Rows);

    private double[] Read(int count)
    {
        var result = new double[count];
        for (var i = 0; i < count; ++i)
        {
            result[i] = _reader.ReadDouble();
        }
        return result;
    }

    public void StartAgain()
    {
        _stream.Seek(_start, SeekOrigin.Begin);
    }
}
