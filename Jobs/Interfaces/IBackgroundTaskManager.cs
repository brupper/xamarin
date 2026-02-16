using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> Manages the upload transfers of the tasks. </summary>
public interface IBackgroundTaskManager
{
    /// <summary> Enqueues a task. </summary>
    /// <param name="task">The task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task EnqueueAsync(IBackgroundTask task);

    /// <summary> Cancels the task. </summary>
    void Abort(IBackgroundTask task);

    /// <summary> Starts the uploading process. </summary>
    /// <returns>The asynchronous operation.</returns>
    Task StartAsync();

    /// <summary> Restores the tasks from the local database and try to execute the next task again. </summary>
    /// <returns>The asynchronous operation.</returns>
    Task RestoreAndTryExecuteNextAsync();

    /// <summary> Restarts the upload tasks. </summary>
    /// <param name="taskIds">The task id enumeration.</param>
    /// <returns>The asynchronous operation.</returns>
    Task RestartTasksAsync(IEnumerable<string> taskIds);

    /// <summary> Delete upload tasks from the local database. </summary>
    void Reset();

    /// <summary> The queue of the upload tasks. </summary>
    IBackgroundTaskQueue TaskQueue { get; }

    /// <summary> Creates a task asynchronously. </summary>
    /// <typeparam name="TU">The generic type of upload tasks.</typeparam>
    /// <typeparam name="T">The generic type of current task's result.</typeparam>
    /// <param name="parameters">The current task's result.</param>
    /// <returns>The created upload task.</returns>
    Task<TU> CreateTaskAsync<TU, T>(T parameters) where TU : class, IBackgroundTask<T>;
}
