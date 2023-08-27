using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TodoApi.Database;
using TodoApi.IntegrationTests.Helpers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.IntegrationTests;

[Collection("Sequential")]
public class TodoItemsEndpointsIntegrationTests
    : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public TodoItemsEndpointsIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    public static IEnumerable<object[]> InvalidTodos => new List<object[]>
{
    new object[] { new TodoItemDto { Content = "", IsComplete = false }, "Content is empty" },
    new object[] { new TodoItemDto { Content = "no", IsComplete = false }, "Content length < 3" }
};

    [Theory]
    [MemberData(nameof(InvalidTodos))]
    public async Task PostTodoWithValidationProblems(TodoItemDto todo, string errorMessage)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/todoitems", todo);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        HttpValidationProblemDetails? problemResult = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.NotNull(problemResult?.Errors);
        Assert.Collection(problemResult.Errors, (error) => Assert.Equal(errorMessage, error.Value.First()));
    }

    [Fact]
    public async Task PostTodoWithValidParameters()
    {
        using (IServiceScope scope = _factory.Services.CreateScope())
        {
            TodoDbContext? db = scope.ServiceProvider.GetService<TodoDbContext>();
            if (db != null && db.TodoItems.Any())
            {
                db.TodoItems.RemoveRange(db.TodoItems);
                await db.SaveChangesAsync();
            }
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/todoitems", new TodoItemDto
        {
            Content = "Test content",
            IsComplete = false
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        TodoItem[]? todos = await _httpClient.GetFromJsonAsync<TodoItem[]>("/todoitems");

        Assert.NotNull(todos);
        Assert.Single(todos);

        Assert.Collection(todos, (todo) =>
        {
            Assert.Equal("Test content", todo.Content);
            Assert.False(todo.IsComplete);
        });
    }
}