namespace TodoApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public bool IsComplete { get; set; }
}
