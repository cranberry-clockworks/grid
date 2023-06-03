using Repository.Database;

namespace Repository;

/// <summary>
/// A product matrix repository.
/// </summary>
internal interface IMatrixRepository
{
    /// <summary>
    /// Creates a new product matrix in the repository.
    /// </summary>
    /// <param name="rows">
    /// The row count of the product matrix.
    /// </param>
    /// <param name="columns">
    /// The column count of the product matrix.
    /// </param>
    /// <param name="hash">
    /// The hash of multiplied matrices.
    /// </param>
    /// <returns>
    /// The id of the new product matrix or the existing one if the repository already contain the product matrix
    /// computed with matrices represented by hash value.
    /// </returns>
    Task<int> CreateAsync(int rows, int columns, string hash);

    /// <summary>
    /// Updates the value of the product matrix.
    /// </summary>
    /// <param name="id">
    /// The id of the matrix.
    /// </param>
    /// <param name="row">
    /// The row index of the value.
    /// </param>
    /// <param name="column">
    /// The column index of the value
    /// </param>
    /// <param name="newValue">
    /// The new value of the cell.
    /// </param>
    void Update(int id, int row, int column, double newValue);

    /// <summary>
    /// Checks if the matrix is computed.
    /// </summary>
    /// <param name="id">
    /// The matrix id to perform check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if matrix is already computed, <see langword="false"/> in other cases, including if there
    /// is no such matrix in the repository.
    /// </returns>
    Task<bool> IsComputedAsync(int id);

    /// <summary>
    /// Removes the product matrix.
    /// </summary>
    /// <param name="id">
    /// The id of the matrix to remove.
    /// </param>
    Task RemoveAsync(int id);

    /// <summary>
    /// Gets computed values of the matrix in the sorted first by rows than by columns.
    /// </summary>
    /// <param name="id">
    /// The id of the matrix
    /// </param>
    Task<IEnumerable<double>> GetComputedValuesAsync(int id);

    /// <summary>
    /// Get the size of the matrix.
    /// </summary>
    /// <param name="id">
    /// The id of the matrix to obtain the size.
    /// </param>
    /// <returns>
    /// The size of the matrix.
    /// </returns>
    Task<MatrixSize?> GetMatrixAsync(int id);
}
