using TodoApi.Models;

namespace TodoApi.Helpers;

public static class Utilities
{
    public static Dictionary<string, string[]> IsValid(TodoItemDto todo)
    {
        Dictionary<string, string[]> errors = new();

        if (string.IsNullOrEmpty(todo.Content))
        {
            errors.TryAdd("todoitem.content.errors", new[] { "Content is empty" });
        }

        if (!string.IsNullOrEmpty(todo.Content) && todo.Content.Length < 3)
        {
            errors.TryAdd("todoitem.content.errors", new[] { "Content length < 3" });
        }

        return errors;
    }
}
