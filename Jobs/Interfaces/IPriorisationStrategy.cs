using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> Provides priorisation strategy for the tasks. </summary>
public interface IPriorisationStrategy
{
    /// <summary> The data source of the priorisation. </summary>
    IPriorityQueue DataSource { get; set; }

    /// <summary> Enqueues the new task asynchronously. </summary>
    /// <param name="newTask">The new task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task EnqueueAsync(IBackgroundTask newTask);

    /// <summary> Handles error tasks. </summary>
    /// <param name="errorTask">The error task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task HandleErrorAsync(IBackgroundTask errorTask);
}
