using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> Manages storing the upload task logs in the local database. </summary>
public interface IBackgroundTaskLogRepository
{
    /// <summary> Saves an upload task log object to the local database. </summary>
    /// <param name="uploadTaskLog">The upload task log.</param>
    /// <returns>The asynchronous operation.</returns>
    Task CreateAsync(BackgroundTaskLog uploadTaskLog);

    /// <summary> Returns an upload task log from the local database. </summary>
    /// <param name="id">The upload task log id.</param>
    /// <returns>The upload task log enumerable belongs to the id.</returns>
    Task<IEnumerable<BackgroundTaskLog>> GetForIdAsync(string id);
}
