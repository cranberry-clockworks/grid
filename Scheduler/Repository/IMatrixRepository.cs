namespace Scheduler.Repository;

/// <summary>
/// Matrix repository.
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
    /// <param name="token">
    /// A token to cancel the operation.
    /// </param>
    /// <returns>
    /// The id of the new product matrix or the existing one if the repository already contain the product matrix
    /// computed with matrices represented by hash value.
    /// </returns>
    Task<int> CreateAsync(int rows, int columns, string hash, CancellationToken token);

    /// <summary>
    /// Checks if the matrix is computed.
    /// </summary>
    /// <param name="id">
    /// The matrix id to perform check.
    /// </param>
    /// <param name="token">
    /// A token to cancel the operation.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if matrix is already computed, <see langword="false"/> in other cases, including if there
    /// is no such matrix in the repository.
    /// </returns>
    Task<bool> IsComputed(int id, CancellationToken token);
}
