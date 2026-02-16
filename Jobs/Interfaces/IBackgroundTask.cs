using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> The base object for uploading audits. </summary>
public interface IBackgroundTask
{
    /// <summary> The task id. </summary>
    string Id { get;  }

    /// <summary> The task status. </summary>
    Brupper.Jobs.TaskStatus Status { get; }

    /// <summary> The subtasks of the task. </summary>
    IEnumerable<IBackgroundTask> Subtasks { get;  }

    /// <summary> The rank. </summary>
    int Rank { get; set; }

    /// <summary> The creation date. </summary>
    DateTime CreationDate { get; set; }

    /// <summary> The last modification date. </summary>
    DateTime LastModificationDate { get; set; }

    /// <summary> Gets the Entity of the task belongs to. </summary>
    string EntityId { get; }

    /// <summary> The task error. </summary>
    TaskErrorCode Error { get; }

    /// <summary> Executes the task asynchronously. </summary>
    Task ExecuteAsync();

    /// <summary> Cancels the task. Note: not deletes itself! It depends on the implementation. </summary>
    void Abort();

    /// <summary> Serializes the task. </summary>
    /// <returns>The serialized string.</returns>
    string Serialize();

    /// <summary> Sets the status of the task. </summary>
    /// <param name="status">The status.</param>
    /// <param name="comment">The comment.</param>
    void SetStatus(Brupper.Jobs.TaskStatus status, string comment);

    /// <summary> Deserializes the current upload task asynchronously. </summary>
    /// <param name="info">The base information to the serialization.</param>
    /// <returns>The asynchronous operation.</returns>
    Task DeserializeAsync(string info);

    /// <summary> Handles specific process related tasks. </summary>
    /// <param name="task">The process we check.</param>
    void ProcessRelatedTasks(IEnumerable<IBackgroundTask> tasks);

    /// <summary>
    /// Determines whether the specified <see cref="IBackgroundTask"/> 
    /// is equal this instance.
    /// </summary>
    /// <param name="task">The <see cref="IBackgroundTask"/> to compare with this instance.</param>
    /// <returns>True if the specified <see cref="IBackgroundTask"/> is equal to this instance otherwise false.</returns>
    bool IsSubjectSame(IBackgroundTask task);

    /// <summary>  </summary>
    bool IsAborted { get; }
}
