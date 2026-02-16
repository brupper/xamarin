using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> The generic extension for tasks to uploading. </summary>
/// <typeparam name="T">The generic type.</typeparam>
public interface IBackgroundTask<T> : IBackgroundTask
{
    /// <summary> Initializes the task asynchronously. </summary>
    /// <param name="param"></param>
    /// <returns>The asynchronous operation.</returns>
    Task InitAsync(T param);
}
