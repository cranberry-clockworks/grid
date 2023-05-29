namespace Repository.Models;

internal class Matrix
{
    public int Id { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public string Hash { get; set; } = string.Empty;
}
