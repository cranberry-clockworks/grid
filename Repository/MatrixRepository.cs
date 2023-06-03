using System.Data;
using Dapper;
using Repository.Database;

namespace Repository;

internal class MatrixRepository : IMatrixRepository
{
    private readonly IDbConnection _connection;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    /// <param name="connection">
    /// A connection to the database.
    /// </param>
    public MatrixRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    /// <inheritdoc />
    public async Task<int> CreateAsync(int rows, int columns, string hash)
    {
        var args = new
        {
            rows,
            columns,
            hash
        };

        var hashed = (
            await _connection.QueryAsync<int>(
                """
                SELECT id from Matricies
                WHERE rows = @Rows AND columns = @Columns AND hash = @Hash;
                """,
                args
            )
        ).ToList();

        if (hashed.Count != 0)
        {
            return hashed.First();
        }

        return _connection
            .Query<int>(
                """
                INSERT INTO Matricies (rows, columns, hash) VALUES
                (@Rows, @Columns, @Hash)
                RETURNING id;
                """,
                args
            )
            .FirstOrDefault();
    }

    /// <inheritdoc />
    public async Task<bool> IsComputedAsync(int id) =>
        (
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

    /// <inheritdoc />
    public async Task RemoveAsync(int id) =>
        await _connection.QueryAsync("DELETE FROM Matricies WHERE id = @id", new { id });

    /// <inheritdoc />
    public async Task UpdateAsync(int id, int row, int column, double newValue) =>
        await _connection.QueryAsync(
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

    /// <inheritdoc />
    public async Task<IEnumerable<double>> GetComputedValuesAsync(int id) =>
        await _connection.QueryAsync<double>(
            """
            SELECT "value"
            FROM Values
            WHERE "id" = @Id
            ORDER BY "row", "column";
            """,
            new { id }
        );

    /// <inheritdoc />
    public async Task<MatrixSize?> GetMatrixAsync(int id) =>
        (
            await _connection.QueryAsync<MatrixSize?>(
                """
            SELECT "rows", "columns"
            FROM Matricies
            WHERE "id" = @Id;
            """,
                new { id }
            )
        ).FirstOrDefault();
}
