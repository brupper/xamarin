using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> The retrieval strategy to getting the next task. </summary>
public interface IRetrievalStrategy
{
    /// <summary> The data source of the retrieval strategy. </summary>
    IPriorityQueue DataSource { get; set; }

    /// <summary> Returns with the next task. </summary>
    /// <returns>The next task.</returns>
    Task<IBackgroundTask> GetNext();
}
