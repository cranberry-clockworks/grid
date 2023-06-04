using System.Data;
using Dapper;
using Repository.Database;

namespace Repository;

internal class MatrixRepository : IMatrixRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger _logger;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="logger">
    /// A logger to write messages.
    /// </param>
    /// <param name="connection">
    /// A connection to the database.
    /// </param>
    public MatrixRepository(ILogger<MatrixRepository> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(int rows, int columns, string hash)
    {
        using var scope = _logger.BeginScope(
            "Creating new product matrix. Rows: {Rows}, Columns: {Columns}, Hash: {Hash}",
            rows,
            columns,
            hash
        );
        var args = new
        {
            rows,
            columns,
            hash
        };

        var cached = (
            await _connection.QueryAsync<int>(
                """
                SELECT id from Matricies
                WHERE rows = @Rows AND columns = @Columns AND hash = @Hash;
                """,
                args
            )
        ).ToList();

        if (cached.Count != 0)
        {
            var cachedId = cached.First();
            _logger.LogDebug("Already existed with given hash. Id: {Id}", cachedId);
            return cachedId;
        }

        var id = _connection
            .Query<int>(
                """
                INSERT INTO Matricies (rows, columns, hash) VALUES
                (@Rows, @Columns, @Hash)
                RETURNING id;
                """,
                args
            )
            .FirstOrDefault();
        _logger.LogDebug("Created. Id: {Id}", id);
        return id;
    }

    /// <inheritdoc />
    public async Task<bool> IsComputedAsync(int id)
    {
        using var scope = _logger.BeginScope("Checking if already computed. Id: {Id}", id);
        var computed = (
            await _connection.QueryAsync(
                """
            SELECT m.id
            FROM Matricies m
            WHERE m.id = @id AND m.rows * m.columns = (
                SELECT COUNT(id)
                FROM Values v
                WHERE v.id = m.id
            );
            """,
                new { id }
            )
        ).Any();
        _logger.LogDebug("Computed: {Result}", computed);
        return computed;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int id) =>
        await _connection.QueryAsync("DELETE FROM Matricies WHERE id = @id", new { id });

    private readonly SemaphoreSlim _semaphore = new(1);

    /// <inheritdoc />
    public void Update(int id, int row, int column, double newValue)
    {
        _logger.LogDebug(
            "Update value. Id: {Id}, Row: {Row}, Column: {Column}, Value: {Value}",
            id,
            row,
            column,
            newValue
        );
        _connection.Query(
            """
                INSERT INTO Values ("id", "row", "column", "value") VALUES
                (@Id, @Row, @Column, @Value)
                ON CONFLICT ("id", "row", "column")
                DO UPDATE SET value = EXCLUDED.value;
            """,
            new
            {
                id,
                row,
                column,
                Value = newValue
            }
        );
    }

    /// <inheritdoc />
    public async Task<IEnumerable<double>> GetComputedValuesAsync(int id)
    {
        _logger.LogDebug("Requesting computed values. Id: {Id}", id);
        return await _connection.QueryAsync<double>(
            """
            SELECT "value"
            FROM Values
            WHERE "id" = @Id
            ORDER BY "row", "column";
            """,
            new { id }
        );
    }

    /// <inheritdoc />
    public async Task<MatrixSize?> GetMatrixAsync(int id)
    {
        using var scope = _logger.BeginScope("Requesting size for matrix. Id: {Id}", id);
        var size = (
            await _connection.QueryAsync<MatrixSize?>(
                """
            SELECT "rows", "columns"
            FROM Matricies
            WHERE "id" = @Id;
            """,
                new { id }
            )
        ).FirstOrDefault();
        _logger.LogDebug("Requested size. Size: {Size}", size);
        return size;
    }
}
