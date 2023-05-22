namespace Scheduler.Repository;

internal interface IMatrixRepository
{
    Guid Create(int rows, int columns);
    Matrix Get(Guid guid);
    void Remove(Guid guid);
}
