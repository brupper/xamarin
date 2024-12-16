using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> Manages storing the upload tasks in the local database. </summary>
public interface IBackgroundTaskRepository
{
    /// <summary> Returns all of the upload tasks from the local database. </summary>
    /// <returns>The upload task enumerable.</returns>
    Task<IEnumerable<BackgroundTask>> GetAllAsync();

    /// <summary> Deletes an upload task from the local database. </summary>
    /// <param name="task">The upload task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task DeleteAsync(BackgroundTask task);

    /// <summary> Deletes all of the upload tasks from the local database.  </summary>
    /// <returns>The asynchronous operation.</returns>
    Task DeleteAllAsync();

    /// <summary> Saves an upload task object to the local database. </summary>
    /// <param name="uploadTask">The upload task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task CreateAsync(BackgroundTask uploadTask);

    /// <summary> Returns an upload task from the local database. </summary>
    /// <param name="id">The upload task id.</param>
    /// <returns>The searched upload task.</returns>
    Task<BackgroundTask> GetAsync(string id);

    /// <summary> Returns upload tasks from the local database. </summary>
    /// <param name="ids">The upload task ids.</param>
    /// <returns>The upload task enumerable.</returns>
    Task<IEnumerable<BackgroundTask>> GetMultipleAsync(IEnumerable<string> ids);

    /// <summary> Updates an upload task in the local database. </summary>
    /// <param name="uploadTask">The upload task.</param>
    /// <returns>The asynchronous operation.</returns>
    Task UpdateAsync(BackgroundTask uploadTask);

    /// <summary> Returns all of the not completed upload tasks from the local database. </summary>
    /// <returns>The upload task enumerable.</returns>
    Task<IEnumerable<BackgroundTask>> GetTasksToPerformAsync();

    /// <summary> Get the list of task belonging to a given UUID with the proper version. </summary>
    /// <param name="uuid">The globally unique id / guid.</param>
    /// <param name="localVersion">The version.</param>
    /// <returns>The upload task enumerable.</returns>
    Task<IEnumerable<BackgroundTask>> GetTasksByVersionAsync(string uuid, int localVersion);
}
