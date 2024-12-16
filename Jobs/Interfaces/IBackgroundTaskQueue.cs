using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> The queue for the tasks. </summary>
public interface IBackgroundTaskQueue
{
    /// <summary> Returns if there is a task in the queue. </summary>
    bool HasTasks { get; }

    /// <summary> The priorisation strategy. </summary>
    IPriorisationStrategy PrioritisationStrategy { get; }

    /// <summary> The task retrieval strategy. </summary>
    IRetrievalStrategy TaskRetrievalStrategy { get; }

    /// <summary> Enqueues a task. </summary>
    /// <param name="task">The task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task Enqueue(IBackgroundTask task);

    /// <summary> Tries to Enqueue a task. </summary>
    /// <param name="task">The task.</param>
    /// <returns>The result is <c>true</c> if the operation is successful otherwise, <c>false</c>.</returns>
    Task<bool> TryEnqueue(IBackgroundTask task);

    /// <summary> Returns with the next task in the queue. </summary>
    /// <returns>The next task is the queue.</returns>
    Task<IBackgroundTask> GetNext();

    /// <summary> Restores the tasks from the local database.</summary>
    /// <returns>The asynchronous operation.</returns>
    Task Restore();

    /// <summary> Handles the errors of the operation. </summary>
    /// <param name="task">The task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task SignalError(IBackgroundTask task);

    /// <summary> Finds a task by id. </summary>
    /// <param name="id">The id.</param>
    /// <returns>The searched task.</returns>
    IBackgroundTask Find(string id);

    /// <summary> Removes a task. </summary>
    /// <param name="task">The task.</param>
    /// <returns>The removed task asynchronously.</returns>
    Task<IBackgroundTask> Remove(IBackgroundTask task);
}
