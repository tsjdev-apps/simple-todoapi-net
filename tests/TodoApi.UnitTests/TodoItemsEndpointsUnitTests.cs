using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Database;
using TodoApi.Endpoints;
using TodoApi.Models;
using TodoApi.UnitTests.Helpers;
using Xunit;

namespace TodoApi.UnitTests;

public class TodoItemsEndpointsUnitTests
{
    [Fact]
    public async Task GetTodoReturnsNotFoundIfNotExists()
    {
        // Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        // Act
        Results<Ok<TodoItem>, NotFound> result = await TodoItemsEndpoints.GetTodoItem(1, context);

        //Assert
        Assert.IsType<Results<Ok<TodoItem>, NotFound>>(result);
        NotFound notFoundResult = (NotFound)result.Result;
        Assert.NotNull(notFoundResult);
    }

    [Fact]
    public async Task GetAllReturnsTodosFromDatabase()
    {
        // Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Content = "Test Content 1",
            IsComplete = false
        });

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Content = "Test Content 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        // Act
        Ok<TodoItem[]> result = await TodoItemsEndpoints.GetAllTodoItems(context);

        //Assert
        Assert.IsType<Ok<TodoItem[]>>(result);

        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
        Assert.Collection(result.Value, todo1 =>
        {
            Assert.Equal(1, todo1.Id);
            Assert.Equal("Test Content 1", todo1.Content);
            Assert.False(todo1.IsComplete);
        }, todo2 =>
        {
            Assert.Equal(2, todo2.Id);
            Assert.Equal("Test Content 2", todo2.Content);
            Assert.True(todo2.IsComplete);
        });
    }

    [Fact]
    public async Task GetCompletedReturnsTodosFromDatabase()
    {
        // Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Content = "Test Content 1",
            IsComplete = false
        });

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Content = "Test Content 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        // Act
        Ok<TodoItem[]> result = await TodoItemsEndpoints.GetCompleteTodoItems(context);

        //Assert
        Assert.IsType<Ok<TodoItem[]>>(result);

        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
        Assert.Single(result.Value);

        Assert.Collection(result.Value, todo1 =>
        {
            Assert.Equal(2, todo1.Id);
            Assert.Equal("Test Content 2", todo1.Content);
            Assert.True(todo1.IsComplete);
        });
    }

    [Fact]
    public async Task GetIncompletedReturnsTodosFromDatabase()
    {
        // Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Content = "Test Content 1",
            IsComplete = false
        });

        context.TodoItems.Add(new TodoItem
        {
            Id = 2,
            Content = "Test Content 2",
            IsComplete = true
        });

        await context.SaveChangesAsync();

        // Act
        Ok<TodoItem[]> result = await TodoItemsEndpoints.GetIncompleteTodoItems(context);

        //Assert
        Assert.IsType<Ok<TodoItem[]>>(result);

        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
        Assert.Single(result.Value);

        Assert.Collection(result.Value, todo1 =>
        {
            Assert.Equal(1, todo1.Id);
            Assert.Equal("Test Content 1", todo1.Content);
            Assert.False(todo1.IsComplete);
        });
    }


    [Fact]
    public async Task GetTodoReturnsTodoFromDatabase()
    {
        // Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Content = "Test Content",
            IsComplete = false
        });

        await context.SaveChangesAsync();

        // Act
        Results<Ok<TodoItem>, NotFound> result = await TodoItemsEndpoints.GetTodoItem(1, context);

        //Assert
        Assert.IsType<Results<Ok<TodoItem>, NotFound>>(result);

        Ok<TodoItem> okResult = (Ok<TodoItem>)result.Result;

        Assert.NotNull(okResult.Value);
        Assert.Equal(1, okResult.Value.Id);
    }

    [Fact]
    public async Task CreateTodoCreatesTodoInDatabase()
    {
        //Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        TodoItemDto newTodo = new()
        {
            Content = "Test Content",
            IsComplete = false
        };

        //Act
        Created<TodoItem> result = await TodoItemsEndpoints.CreateTodoItem(newTodo, context);

        //Assert
        Assert.IsType<Created<TodoItem>>(result);

        Assert.NotNull(result);
        Assert.NotNull(result.Location);

        Assert.NotEmpty(context.TodoItems);
        Assert.Collection(context.TodoItems, todo =>
        {
            Assert.Equal("Test Content", todo.Content);
            Assert.False(todo.IsComplete);
        });
    }

    [Fact]
    public async Task UpdateTodoUpdatesTodoInDatabase()
    {
        //Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        context.TodoItems.Add(new TodoItem
        {
            Id = 1,
            Content = "Exiting test content",
        });

        await context.SaveChangesAsync();

        TodoItemDto updatedTodo = new()
        {
            Content = "Updated test content",
            IsComplete = true
        };

        //Act
        Results<NoContent, NotFound> result = await TodoItemsEndpoints.UpdateTodoItem(1, updatedTodo, context);

        //Assert
        Assert.IsType<Results<NoContent, NotFound>>(result);

        NoContent noContentResult = (NoContent)result.Result;

        Assert.NotNull(noContentResult);

        TodoItem? todoInDb = await context.TodoItems.FindAsync(1);

        Assert.NotNull(todoInDb);
        Assert.Equal("Updated test content", todoInDb!.Content);
        Assert.True(todoInDb.IsComplete);
    }

    [Fact]
    public async Task DeleteTodoDeletesTodoInDatabase()
    {
        //Arrange
        await using TodoDbContext context = new MockDatabase().CreateDbContext();

        TodoItem existingTodo = new()
        {
            Id = 1,
            Content = "Exiting test content",
            IsComplete = false
        };

        context.TodoItems.Add(existingTodo);

        await context.SaveChangesAsync();

        //Act
        Results<NoContent, NotFound> result = await TodoItemsEndpoints.DeleteTodoItem(existingTodo.Id, context);

        //Assert
        Assert.IsType<Results<NoContent, NotFound>>(result);

        NoContent noContentResult = (NoContent)result.Result;

        Assert.NotNull(noContentResult);
        Assert.Empty(context.TodoItems);
    }
}