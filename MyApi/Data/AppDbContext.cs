using Microsoft.EntityFrameworkCore;

namespace MyApi.Data;   // <- must match your using in Program.cs

using Microsoft.EntityFrameworkCore;
using MyApi.Models;     // needed if DbSet<Todo> is used

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos => Set<Todo>();
}
