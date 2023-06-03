namespace Scheduler.Scheduler;

/// <summary>
/// A matrix multiplication task scheduler.
/// </summary>
internal interface IProductTaskScheduler
{
    /// <summary>
    /// Reads matrices and subdivides them on task to product matrices in parallel. Schedules the created tasks into
    /// the processing queue.
    /// </summary>
    /// <remarks>
    /// Since the matrix product is not commutative operation the result is assumed as
    /// <c>first * second = matrixId</c>
    /// </remarks>
    /// <param name="matrixId">
    /// The id of the product matrix.
    /// </param>
    /// <param name="first">
    /// The first matrix production operand.
    /// </param>
    /// <param name="second">
    /// The second matrix production operand.
    /// </param>
    Task ScheduleAsync(int matrixId, MatrixStreamReader first, MatrixStreamReader second);
}
