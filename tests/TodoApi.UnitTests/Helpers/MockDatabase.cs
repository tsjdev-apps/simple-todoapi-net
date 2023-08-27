using Microsoft.EntityFrameworkCore;
using TodoApi.Database;

namespace TodoApi.UnitTests.Helpers;

public class MockDatabase : IDbContextFactory<TodoDbContext>
{
    public TodoDbContext CreateDbContext()
    {
        DbContextOptions<TodoDbContext> options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        return new TodoDbContext(options);
    }
}
