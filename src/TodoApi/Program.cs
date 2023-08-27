using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Database;
using TodoApi.Endpoints;

// create webapplicationbuilder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// set port
builder.WebHost.UseUrls(new[] { "http://0.0.0.0:5097", "https://0.0.0.0:5098" });

// configure database
builder.Services.AddDbContext<TodoDbContext>(options =>
{
    var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    options.UseSqlite($"Data Source={Path.Join(path, "TodoApi.db")}");
});

// configure swagger and endpointsapiexplorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        Contact = new OpenApiContact
        {
            Name = "Thomas Sebastian Jensen",
            Url = new Uri("https://www.tsjdev-apps.de")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });
});

// build webapplication
WebApplication app = builder.Build();

// enable swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// run database migration
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<TodoDbContext>();
db?.Database.MigrateAsync();

// group endpoints
app.MapGroup("/todoitems")
    .MapTodoItemsEndpoints()
    .WithTags("Todo Items Endpoints");

app.Run();


public partial class Program { }