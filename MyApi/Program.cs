using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using MyApi.Services;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Only register SQLite if not in integration test
        var isIntegrationTest = builder.Environment.EnvironmentName == "IntegrationTests";

        if (!isIntegrationTest)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=todos.db"));
        }

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // GET /todos - return all todos
        app.MapGet("/todos", async (AppDbContext db) =>
            Results.Ok(await db.Todos.ToListAsync()));

        // GET /todos/{id} - return single todo or 404
        app.MapGet("/todos/{id}", async (int id, AppDbContext db) =>
        {
            var todo = await db.Todos.FindAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        });

        // POST /todos - create a new todo with validation
        app.MapPost("/todos", async (Todo todo, AppDbContext db, ILogger<Program> logger) =>
        {
            if (!TodoService.IsValidTitle(todo.Title))
            {
                logger.LogWarning("Invalid title received: {Title}", todo.Title);
                return Results.BadRequest(new { error = "Title cannot be empty." });
            }

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            return Results.Created($"/todos/{todo.Id}", todo);
        });

        // PUT /todos/{id} - update existing todo or 404 if not found
        app.MapPut("/todos/{id}", async (int id, Todo updated, AppDbContext db) =>
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return Results.NotFound();

            if (!TodoService.IsValidTitle(updated.Title))
                return Results.BadRequest(new { error = "Title cannot be empty." });

            todo.Title = updated.Title;
            todo.Completed = updated.Completed;

            await db.SaveChangesAsync();
            return Results.Ok(todo);
        });

        // DELETE /todos/{id} - delete existing todo or 404 if not found
        app.MapDelete("/todos/{id}", async (int id, AppDbContext db) =>
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return Results.NotFound();

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.Run();
    }
}
