using Repository.Database;

namespace Repository;

internal interface IMatrixRepository
{
    Task<int> CreateAsync(int rows, int columns, string hash);
    void Update(int id, int row, int column, double newValue);
    Task<bool> IsComputedAsync(int id);
    Task RemoveAsync(int id);
    Task<IEnumerable<double>> GetComputedValuesAsync(int id);
    Task<MatrixSize?> GetMatrixAsync(int id);
}
