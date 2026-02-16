using Microsoft.EntityFrameworkCore;

namespace Brupper.Jobs.Contexts;

public class BackgroundTasksDbContext(DbContextOptions<BackgroundTasksDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BackgroundTaskLog>().HasNoKey();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<BackgroundTask> BackgroundTasks { get; set; } = default!;

    public DbSet<BackgroundTaskLog> BackgroundTaskLogs { get; set; } = default!;

}
