using Brupper.Jobs.Contexts;
using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.Interfaces.ITransferManager"/> implementation. </summary>
public class BackgroundTaskManager : IBackgroundTaskManager
{
    #region Static Properties

    private static IBackgroundTaskManager instance;
    private static bool isRunning;

    #endregion

    #region Private Properties

    private readonly IBackgroundTaskRepository uploadTaskLocalService;
    private readonly IDbContextFactory<BackgroundTasksDbContext> dbContextFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;

    #endregion

    #region Public Properties

    /// <summary> The queue of the upload tasks. </summary>
    public IBackgroundTaskQueue TaskQueue { get; private set; }

    #endregion

    #region Constructors

    /// <summary> Initializes a new instance of the <see cref="Brupper.Jobs.Implementations.TransferManager"/> class. </summary>
    /// <param name="taskQueue">The task queue.</param>
    public BackgroundTaskManager(
        IDbContextFactory<BackgroundTasksDbContext> dbContextFactory,
        IBackgroundTaskRepository uploadTaskLocalService,
        IServiceProvider serviceProvider,
        ILogger<BackgroundTaskManager> logger)
    {
        Guard.IsNotNull(dbContextFactory, nameof(dbContextFactory));
        Guard.IsNotNull(uploadTaskLocalService, nameof(uploadTaskLocalService));
        Guard.IsNotNull(logger, nameof(logger));

        this.dbContextFactory = dbContextFactory;
        this.uploadTaskLocalService = uploadTaskLocalService;
        this.serviceProvider = serviceProvider;
        this.logger = logger;

        Reset();
    }

    #endregion

    #region ITransferManager implementation

    public void Abort(IBackgroundTask task)
    {
        Guard.IsNotNull(task, nameof(task));

        task.Abort();
    }

    public async Task EnqueueAsync(IBackgroundTask task)
    {
        Guard.IsNotNull(task, nameof(task));
        Guard.IsNotNullOrEmpty(task.Id, nameof(task.Id));

        if (await TaskQueue.TryEnqueue(task))
        {
            ExecuteAsync().FireAndForget();
        }
    }

    public void Reset()
    {
        TaskQueue = new BackgroundTaskQueue(new PriorityQueue(uploadTaskLocalService, serviceProvider, logger));
    }

    public async Task RestartTasksAsync(IEnumerable<string> taskIds)
    {
        foreach (var id in taskIds)
        {
            var task = TaskQueue.Find(id);

            if (task != null && task.Status == Brupper.Jobs.TaskStatus.Error)
            {
                await TaskQueue.Remove(task);
                task.SetStatus(Brupper.Jobs.TaskStatus.Pending, "Manual task restart");
                await TaskQueue.TryEnqueue(task);
            }
        }

        ExecuteAsync().FireAndForget();
    }

    public async Task RestoreAndTryExecuteNextAsync()
    {
        await TaskQueue.Restore();

        await TryExecuteNextAsync();
    }

    public async Task StartAsync()
    {
        using (var sqliteService = dbContextFactory.CreateDbContext())
        {
            await sqliteService.Database.EnsureCreatedAsync();
        }

        await TaskQueue.Restore();

        ExecuteAsync().FireAndForget();
    }

    #endregion

    #region Private Methods

    private async Task ExecuteAsync()
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        var tasks = TaskQueue;

        await Task.Run(async () =>
        {
            IBackgroundTask nextTask = null;

            do
            {
                nextTask = await tasks.GetNext();

                if (nextTask != null)
                {
                    await ExecuteAsync(nextTask);
                }
            }
            while (nextTask != null);
        });

        isRunning = false;
    }

    private async Task ExecuteAsync(IBackgroundTask task)
    {
        try
        {
            /*
            var networkContext = networkContextProvider.GetContext();

            if (!networkContext.IsNetworkStableEnough)
            {
                Logger.Current?.TrackTrace(LogTag.NetworkConnection, "No connection");
                task.SetStatus(Enums.TaskStatus.Error, "No connection");
                task.LastModificationDate = DateTime.UtcNow;
                return;
            }
            // */

            /*
            var currentAudit = await itemRepository.GetLocalAsync(task.Audit);
            if (currentAudit != null && currentAudit.IsReopened)
            {
                Logger.Current?.TrackTrace(LogTag.None, "Cannot complete reopened upload task, audit is still in reopened state");
                task.SetStatus(Enums.TaskStatus.Error, "Cannot complete reopened upload task, audit is still in reopened state");
                task.LastModificationDate = DateTime.UtcNow;
                return;
            }
            // */

            await task.ExecuteAsync();
        }
        catch (Exception e)
        {
            logger.LogTrace(LogTags.Error.AsEventId(), $"Exception type: {e.GetType()} / {e.Message}");
            logger.LogError($"{e}");

        }

        if (task.Status == Brupper.Jobs.TaskStatus.Error)
        {
            await TaskQueue.SignalError(task);
        }
        else if (task.Status == Brupper.Jobs.TaskStatus.Completed)
        {
            foreach (var subtask in task.Subtasks)
            {
                await EnqueueAsync(subtask);
            }

            await TaskQueue.Remove(task);
        }
    }

    private async Task TryExecuteNextAsync()
    {
        var nextTask = await TaskQueue.GetNext();

        if (nextTask != null)
        {
            await ExecuteAsync(nextTask);
        }
    }

    public async Task<TU> CreateTaskAsync<TU, T>(T parameters) where TU : class, IBackgroundTask<T>
    {
        var task = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<TU>(serviceProvider);

        await task.InitAsync(parameters);

        task.SetStatus(TaskStatus.Pending, "Creating task");

        return task;
    }

    #endregion
}
