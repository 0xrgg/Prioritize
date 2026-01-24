using Microsoft.EntityFrameworkCore;
using Prioritize.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItemEntity> Tasks => Set<TaskItemEntity>();
}


