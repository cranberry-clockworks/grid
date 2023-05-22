namespace Scheduler.Repository;

internal class MatrixRepository : IMatrixRepository
{
    private readonly Dictionary<Guid, Matrix> _matricies = new();

    public Guid Create(int rows, int columns)
    {
        var matrix = new Matrix(rows, columns);
        var guid = Guid.NewGuid();

        _matricies.Add(guid, matrix);
        return guid;
    }

    public Matrix Get(Guid guid) => _matricies[guid];

    public void Remove(Guid guid) => _matricies.Remove(guid);
}
