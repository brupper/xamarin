using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IBackgroundTaskQueue"/> implementation. </summary>
public class BackgroundTaskQueue(PriorityQueue queue/*, ILogger<TaskQueue> logger*/) : IBackgroundTaskQueue
{
    #region Private Properties

    private readonly PriorityQueue internalQueue = queue;
    //private readonly ILogger logger = logger;

    private IPriorisationStrategy prioritisationStrategy = new PriorisationStrategy() { DataSource = queue };
    private IRetrievalStrategy taskRetrievalStrategy = new RetrievalStrategy() { DataSource = queue };

    #endregion

    #region IBackgroundTaskQueue implementation

    public bool HasTasks => internalQueue.Count > 0;

    public IPriorisationStrategy PrioritisationStrategy => prioritisationStrategy;
    public IRetrievalStrategy TaskRetrievalStrategy => taskRetrievalStrategy;

    public Task Enqueue(IBackgroundTask task)
    {
        return PrioritisationStrategy.EnqueueAsync(task);
    }

    public IBackgroundTask Find(string id)
    {
        return internalQueue.Find(id);
    }

    public Task<IBackgroundTask> GetNext()
    {
        return TaskRetrievalStrategy.GetNext();
    }

    public Task<IBackgroundTask> Remove(IBackgroundTask task)
    {
        return internalQueue.Remove(task);
    }

    public async Task Restore()
    {
        if (!HasTasks)
        {
            await internalQueue.Restore();
        }
    }

    public async Task SignalError(IBackgroundTask task)
    {
        await PrioritisationStrategy.HandleErrorAsync(task);
    }

    public async Task<bool> TryEnqueue(IBackgroundTask task)
    {
        if (internalQueue.Contains(task))
        {
            // Logger.Current.TrackTrace(LogTag.TaskChangeEvent, string.Format($"Task {task.Id} already present in queue"));

            return false;
        }

        await Enqueue(task);

        return true;
    }

    #endregion
}
