namespace Repository.Models;

/// <summary>
/// Options to create a new product matrix.
/// </summary>
/// <param name="Rows">
/// Number of rows of the computed matrix.
/// </param>
/// <param name="Columns">
/// Number of columns of the computed matrix.
/// </param>
/// <param name="Hash">
/// The hash of the multiplied matrices.
/// </param>
internal record MatrixCreationOptions(int Rows, int Columns, string Hash);
