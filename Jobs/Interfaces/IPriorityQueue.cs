using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary>
/// The queue for the task priorisation.
/// </summary>
public interface IPriorityQueue : IEnumerable<IBackgroundTask>
{
    /// <summary>
    /// Task count in the queue.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Enqueues a task to the top.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task EnqueueOnTop(IBackgroundTask task);

    /// <summary>
    /// Enqueues a task to the end.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task Enqueue(IBackgroundTask task);

    /// <summary>
    /// Dequeues the next task from the queue.
    /// </summary>
    /// <returns>The dequeued task asynchronously.</returns>
    Task<IBackgroundTask> Dequeue();

    /// <summary>
    /// Enqueues the new task before the selected task, <paramref name="task"/>.
    /// </summary>
    /// <param name="task">The selected task.</param>
    /// <param name="newTask">The new task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task AddBefore(IBackgroundTask task, IBackgroundTask newTask);

    /// <summary>
    /// Enqueues the new task after the selected task, <paramref name="task"/>.
    /// </summary>
    /// <param name="task">The selected task</param>
    /// <param name="newTask">The new task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task AddAfter(IBackgroundTask task, IBackgroundTask newTask);

    /// <summary>
    /// Removes a task.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>The removed task asynchronously.</returns>
    Task<IBackgroundTask> Remove(IBackgroundTask task);

    /// <summary>
    /// Returns the first task in the queue.
    /// </summary>
    /// <returns>The first task in the queue.</returns>
    IBackgroundTask PeekNext();

    /// <summary>
    /// Returns the last task in the queue.
    /// </summary>
    /// <returns>The last task in the queue.</returns>
    IBackgroundTask PeekLast();

    /// <summary>
    /// Finds a task by id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>The searched task.</returns>
    IBackgroundTask Find(string id);

    /// <summary>
    /// Checks if the queue contains a task or not.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>The result is <c>true</c> if the queue contains the searched task otherwise, <c>false</c>.</returns>
    bool Contains(IBackgroundTask task);

    /// <summary>
    /// Restores the tasks from the local database.
    /// </summary>
    /// <returns>The asynchronous operation.</returns>
    Task Restore();

    /// <summary>
    /// Returns a new list of the Task list to avoid concurrent list modification. 
    /// </summary>
    /// <returns>The ITask list.</returns>
    IEnumerable<IBackgroundTask> GetItems();
}
