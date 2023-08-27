using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoApi.Database;
using TodoApi.Helpers;
using TodoApi.Models;

namespace TodoApi.Endpoints;

public static class TodoItemsEndpoints
{
    public static RouteGroupBuilder MapTodoItemsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllTodoItems)
            .WithOpenApi();

        group.MapGet("/complete", GetCompleteTodoItems)
            .WithOpenApi();

        group.MapGet("/incomplete", GetIncompleteTodoItems)
            .WithOpenApi();

        group.MapGet("/{id}", GetTodoItem)
            .WithOpenApi();

        group.MapPost("/", CreateTodoItem)
            .AddEndpointFilter(async (efiContext, next) =>
            {
                TodoItemDto param = efiContext.GetArgument<TodoItemDto>(0);
                Dictionary<string, string[]> validationErrors = Utilities.IsValid(param);

                if (validationErrors.Any())
                {
                    return Results.ValidationProblem(validationErrors);
                }

                return await next(efiContext);
            })
            .WithOpenApi();

        group.MapPut("/{id}", UpdateTodoItem)
            .Accepts<TodoItem>("application/json")
            .WithOpenApi();

        group.MapDelete("/{id}", DeleteTodoItem)
            .WithOpenApi();

        return group;
    }

    public static async Task<Ok<TodoItem[]>> GetAllTodoItems(TodoDbContext db)
    {
        return TypedResults.Ok(await db.TodoItems.ToArrayAsync());
    }

    public static async Task<Ok<TodoItem[]>> GetCompleteTodoItems(TodoDbContext db)
    {
        return TypedResults.Ok(await db.TodoItems.Where(t => t.IsComplete).ToArrayAsync());
    }

    public static async Task<Ok<TodoItem[]>> GetIncompleteTodoItems(TodoDbContext db)
    {
        return TypedResults.Ok(await db.TodoItems.Where(t => !t.IsComplete).ToArrayAsync());
    }

    public static async Task<Results<Ok<TodoItem>, NotFound>> GetTodoItem(int id, TodoDbContext db)
    {
        return await db.TodoItems.FindAsync(id)
            is TodoItem todo
                ? TypedResults.Ok(todo)
                : TypedResults.NotFound();
    }

    public static async Task<Created<TodoItem>> CreateTodoItem(TodoItemDto todoDto, TodoDbContext db)
    {
        TodoItem todoItem = new()
        {
            Content = todoDto.Content,
            IsComplete = todoDto.IsComplete
        };

        db.TodoItems.Add(todoItem);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItem);
    }

    public static async Task<Results<NoContent, NotFound>> UpdateTodoItem(int id, TodoItemDto inputTodo, TodoDbContext db)
    {
        TodoItem? todo = await db.TodoItems.FindAsync(id);

        if (todo is null)
        {
            return TypedResults.NotFound();
        }

        todo.Content = inputTodo.Content;
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteTodoItem(int id, TodoDbContext db)
    {
        if (await db.TodoItems.FindAsync(id) is TodoItem todo)
        {
            db.TodoItems.Remove(todo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}
