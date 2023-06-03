namespace Repository.Models;

/// <summary>
/// Returns the computational state of the matrix product.
/// </summary>
/// <param name="MatrixId">
/// The id of the matrix.
/// </param>
/// <param name="IsComputed">
/// <see langowrd="true"/> if the product fully computed, <see langword="false"/> otherwise.
/// </param>
internal record ComputationState(int MatrixId, bool IsComputed);
