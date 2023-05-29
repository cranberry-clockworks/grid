namespace Repository;

internal interface IMatrixRepository
{
    int Create(int rows, int columns, string hash);
    void Update(int id, int row, int column, double newValue);
    bool IsComputed(int id);
    void Remove(int id);
}
