using System.Diagnostics;
using Protocol;
using Scheduler.Repository;

namespace Scheduler.Scheduler;

internal class ProductTaskScheduler : IProductTaskScheduler
{
    private readonly ILogger _logger;
    private readonly IMatrixRepository _repository;
    private readonly IProducer<ComputeTaskKey, ComputeTaskValue> _producer;

    /// <summary>
    /// Creates the task scheduler.
    /// </summary>
    /// <param name="logger">
    /// A logger instance to write messages.
    /// </param>
    /// <param name="repository">
    /// The matrix repository.
    /// </param>
    /// <param name="producer">
    /// The item queue producer to schedule created tasks.
    /// </param>
    public ProductTaskScheduler(
        ILogger<ProductTaskScheduler> logger,
        IMatrixRepository repository,
        IProducer<ComputeTaskKey, ComputeTaskValue> producer
    )
    {
        _logger = logger;
        _repository = repository;
        _producer = producer;
    }

    public async Task ScheduleAsync(
        int matrixId,
        MatrixStreamReader first,
        MatrixStreamReader second
    )
    {
        Debug.Assert(first.Columns == second.Rows);

        var rows = first.Rows;
        var columns = second.Columns;

        for (var row = 0; row < rows; ++row)
        {
            var tasks = new List<Task>(columns);

            // reading rows and columns should be in sync with indexes
            var rowValues = first.ReadRow();

            for (var column = 0; column < columns; ++column)
            {
                var columnValues = second.ReadColumn();

                tasks.Add(ScheduleSingleValueAsync(matrixId, row, column, rowValues, columnValues));
            }

            second.StartAgain();

            await Task.WhenAll(tasks);
        }
    }

    private async Task ScheduleSingleValueAsync(
        int matrixId,
        int row,
        int column,
        double[] rowValues,
        double[] columnValues
    )
    {
        Debug.Assert(rowValues.Length == columnValues.Length);

        var key = new ComputeTaskKey()
        {
            MatrixId = matrixId,
            Row = row,
            Column = column,
        };

        var value = new ComputeTaskValue() { Row = rowValues, Column = columnValues, };

        await _producer.ProduceAsync(key, value, CancellationToken.None);
    }
}
