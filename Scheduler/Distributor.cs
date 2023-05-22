using System.Diagnostics;
using Protocol;
using Scheduler.Repository;

namespace Scheduler;

internal class Distributor
{
    private readonly ILogger _logger;
    private readonly IMatrixRepository _repository;
    private readonly IJobProducer _producer;

    public Distributor(
        ILogger<Distributor> logger,
        IMatrixRepository repository,
        IJobProducer producer
    )
    {
        _logger = logger;
        _repository = repository;
        _producer = producer;
    }

    public async Task<Guid> ScheduleAsync(MatrixStreamReader first, MatrixStreamReader second)
    {
        Debug.Assert(first.Columns == second.Rows);

        var rows = first.Rows;
        var columns = second.Columns;

        var id = _repository.Create(rows, columns);

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

        return id;
    }

    private async Task ScheduleSingleValueAsync(
        Guid guid,
        int row,
        int column,
        double[] rowValues,
        double[] columnValues
    )
    {
        Debug.Assert(rowValues.Length == columnValues.Length);

        var description = new Description()
        {
            JobId = guid,
            Row = row,
            Column = column,
        };

        var payload = new Payload() { Row = rowValues, Column = columnValues, };

        await _producer.PublishAsync(description, payload);
    }
}
