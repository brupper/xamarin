using Brupper.Jobs.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IBackgroundTaskLogRepository"/> implementation. </summary>
public class BackgroundTaskLogRepository(IDbContextFactory<BackgroundTasksDbContext> dbContextFactory) : IBackgroundTaskLogRepository
{
    private readonly IDbContextFactory<BackgroundTasksDbContext> dbContextFactory = dbContextFactory;

    #region IUploadTaskLogLocalService implementation

    public async Task CreateAsync(BackgroundTaskLog uploadTaskLog)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            sqliteService.BackgroundTaskLogs.Add(uploadTaskLog);
            await sqliteService.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<BackgroundTaskLog>> GetForIdAsync(string id)
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            return await sqliteService.BackgroundTaskLogs.Where(x => x.TaskId == id).ToListAsync();
        }
    }

    #endregion
}
