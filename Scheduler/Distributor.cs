using System.Diagnostics;
using Protocol;

namespace Scheduler;

internal class Distributor
{
    private readonly ILogger _logger;
    private readonly IMatrixRepository _repository;
    private readonly IProducer<ComputeTaskKey, ComputeTaskValue> _producer;

    public Distributor(
        ILogger<Distributor> logger,
        IMatrixRepository repository,
        IProducer<ComputeTaskKey, ComputeTaskValue> producer
    )
    {
        _logger = logger;
        _repository = repository;
        _producer = producer;
    }

    public async Task ScheduleAsync(int id, MatrixStreamReader first, MatrixStreamReader second)
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

                tasks.Add(ScheduleSingleValueAsync(id, row, column, rowValues, columnValues));
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
