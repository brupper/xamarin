using Brupper.Jobs.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IBackgroundTaskRepository"/> implementation. </summary>
public class BackgroundTaskRepository : IBackgroundTaskRepository
{
    private readonly IDbContextFactory<BackgroundTasksDbContext> dbContextFactory;

    #region Constructor

    public BackgroundTaskRepository(IDbContextFactory<BackgroundTasksDbContext> dbContextFactory)
    {
        this.dbContextFactory = dbContextFactory;
    }

    #endregion

    #region IUploadTaskLocalService implementation

    public async Task CreateAsync(BackgroundTask uploadTask)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            sqliteService.BackgroundTasks.Add(uploadTask);
            await sqliteService.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            sqliteService.BackgroundTasks.RemoveRange(sqliteService.BackgroundTasks.ToList());
            await sqliteService.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(BackgroundTask task)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            sqliteService.BackgroundTasks.Remove(task);
            await sqliteService.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<BackgroundTask>> GetAllAsync()
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            return await sqliteService.BackgroundTasks.ToListAsync();
        }
    }

    public async Task<BackgroundTask> GetAsync(string id)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            return await sqliteService.BackgroundTasks.FirstOrDefaultAsync(t => t.TaskId == id);
        }
    }

    public async Task<IEnumerable<BackgroundTask>> GetMultipleAsync(IEnumerable<string> ids)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            var tasks = await sqliteService.BackgroundTasks
                .Where(x => ids.Contains(x.TaskId))
                .ToListAsync();
            return tasks;
        }
    }

    public async Task<IEnumerable<BackgroundTask>> GetTasksToPerformAsync()
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            return (await sqliteService.BackgroundTasks.ToListAsync())
                .Where(t => t.Status != TaskStatus.Completed)
                .OrderBy(t => t.Rank)
                .ToList();
        }
    }

    public async Task UpdateAsync(BackgroundTask uploadTask)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            uploadTask.ModifiedAt = DateTime.UtcNow;
            sqliteService.BackgroundTasks.Attach(uploadTask);
            await sqliteService.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<BackgroundTask>> GetTasksByVersionAsync(string uuid, int localVersion)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            var tasks = await sqliteService.BackgroundTasks
                .Where(x => x.ItemId == uuid && x.TaskId.Contains($"_v{localVersion}"))
                .ToListAsync();
            return tasks;
        }
    }

    #endregion
}
