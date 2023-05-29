using System.Data;
using Dapper;
using Repository.Models;

namespace Repository;

internal class MatrixRepository : IMatrixRepository
{
    private readonly IDbConnection _connection;

    public MatrixRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> CreateAsync(int rows, int columns, string hash)
    {
        var args = new
        {
            Rows = rows,
            Columns = columns,
            Hash = hash
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

    public bool IsComputed(int id) =>
        _connection
            .Query(
                """
            SELECT m.id
            FROM Matricies m
            WHERE m.id = @id AND m.rows * m.columns = (
                SELECT COUNT(id)
                FROM Values v
                WHERE v.id = m.id
            );
            """,
                new { Id = id }
            )
            .Any();

    public void Remove(int id) =>
        _connection.Query("DELETE FROM Matricies WHERE id = @id", new { Id = id });

    public void Update(int id, int row, int column, double newValue) =>
        _connection.Query(
            """
                INSERT INTO Values ("id", "row", "column", "value") VALUES
                (@Id, @Row, @Column, @Value)
                ON CONFLICT ("id", "row", "column")
                DO UPDATE SET value = EXCLUDED.value;
            """,
            new
            {
                Id = id,
                Row = row,
                Column = column,
                Value = newValue
            }
        );
}
