namespace Scheduler.Repository;

internal class Matrix
{
    public int ComputedCount => _computedCount.Value;

    public bool IsComputed => _computedCount.Value == _matrix.Length;

    public double[,] Values
    {
        get
        {
            if (!IsComputed)
            {
                throw new InvalidOperationException("Matrix is not completely computed");
            }
            return _matrix;
        }
    }

    private double[,] _matrix;

    private bool[,] _computed;

    private Lazy<int> _computedCount;

    public Matrix(int rows, int columns)
    {
        _matrix = new double[rows, columns];
        _computed = new bool[rows, columns];
        _computedCount = CreateCache();
    }

    /// <exception cref="IndexOutOfRangeException"/>
    public void SetValue(int row, int column, double value)
    {
        if (row < 0 || row > _matrix.GetLength(0))
        {
            throw new IndexOutOfRangeException(nameof(row));
        }
        if (column < 0 || column > _matrix.GetLength(1))
        {
            throw new IndexOutOfRangeException(nameof(column));
        }

        _matrix[row, column] = value;
        _computed[row, column] = true;
        _computedCount = CreateCache();
    }

    private Lazy<int> CreateCache() => new Lazy<int>(CountComputedValues, false);

    private int CountComputedValues()
    {
        var count = 0;
        for (var row = 0; row < _computed.GetLength(0); ++row)
        {
            for (var column = 0; column < _computed.GetLength(1); ++column)
            {
                if (_computed[row, column])
                {
                    count++;
                }
            }
        }
        return count;
    }
}
