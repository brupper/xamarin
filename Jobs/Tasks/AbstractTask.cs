using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Jobs.Tasks;

/// <summary>
/// <see cref="Brupper.Jobs.IBackgroundTask"/> implementation.
/// This is the base class of the background tasks.
/// </summary>
public abstract class AbstractTask : IBackgroundTask
{
    #region Private Properties

    private readonly IBackgroundTaskLogRepository taskRepository;
    private readonly IBackgroundTaskRepository taskLogRepository;

    protected readonly ILogger logger;

    protected IProgress<double> progress;
    protected readonly CancellationTokenSource taskRunningCancellationToken;

    protected bool HasUnrecoverableError =>
        Error == TaskErrorCode.RemoteCreation || Error == TaskErrorCode.CreationForbidden
        || Error == TaskErrorCode.DeserializationError
        ;

    protected CancellationToken UploadTaskCancellationToken => taskRunningCancellationToken.Token;

    // TODO: private IEnumerable<string> taskIds = Enumerable.Empty<string>();

    private string entityId;
    public string EntityId
    {
        get => entityId;
        set
        {
            entityId = value;
            //TODO: taskIds = RelatedTasks.GetAllTaskId(value);
        }
    }

    #endregion

    #region Constructors

    protected AbstractTask(
        IBackgroundTaskLogRepository taskRepository,
        IBackgroundTaskRepository taskLogRepository,
        ILogger logger)
    {
        Guard.IsNotNull(taskRepository, nameof(taskRepository));
        Guard.IsNotNull(taskLogRepository, nameof(taskLogRepository));
        Guard.IsNotNull(logger, nameof(taskLogRepository));

        this.taskLogRepository = taskLogRepository;
        this.taskRepository = taskRepository;
        this.logger = logger;

        var now = DateTime.UtcNow;
        CreationDate = now;
        LastModificationDate = now;

        taskRunningCancellationToken = new();

        progress = new Progress<double>((p) => _ = OnProgressAsync(p));
    }

    #endregion

    #region ITask implementation

    public string Id { get; set; }

    public TaskStatus Status { get; private set; } = TaskStatus.Pending;

    public IEnumerable<IBackgroundTask> Subtasks { get; set; } = [];

    public int Rank { get; set; } = -1;

    public DateTime CreationDate { get; set; }

    public DateTime LastModificationDate { get; set; }

    public TaskErrorCode Error { get; protected set; }

    public bool IsAborted => taskRunningCancellationToken.IsCancellationRequested;

    public virtual void Abort()
    {
        logger.LogTrace(new EventId(3, nameof(Abort)), $"The task {Id} cancelling.");

        taskRunningCancellationToken.Cancel();

        foreach (var subtask in Subtasks)
        {
            subtask.Abort();
        }
    }

    public abstract Task DeserializeAsync(string info);

    public async Task ExecuteAsync()
    {
        logger.LogTrace(LogTags.Other.AsEventId(), $"The task {Id} is starting.");

        if (taskRunningCancellationToken.IsCancellationRequested)
        {
            SetStatus(TaskStatus.Completed, "Task Cancelled");
            logger.LogTrace((LogTags.TaskChangeEvent & LogTags.CanceledAction).AsEventId(), $"The task {Id} is cancelled.");

            return;
        }

        Exception exception = null;

        SetStatus(TaskStatus.Running, "Starting task");
        progress.ReportProgress(0);
        LastModificationDate = DateTime.UtcNow;
        await taskLogRepository.UpdateAsync(new BackgroundTask(this));

        try
        {
            await InternalExecuteAsync();
            progress.ReportProgress(100);
        }
        catch (Exception e)
        {
            if (Error == default) Error = TaskErrorCode.Unknown;

            // Logger.Current?.TrackTrace(LogTag.Error, LoggingLabels.UnexpectedErrorLabel, new Metadata("Error Type", e.GetType()));

            SetStatus(TaskStatus.Error, "Error: " + e.Message);
            exception = e;

            if (!(e is WasNotUploadedToServerException) && !(e is IsNullDuringUploadException))
            {
                logger.LogError($"{e}");
            }
            else
            {
                exception = null;
            }
        }

        LastModificationDate = DateTime.UtcNow;
        await taskLogRepository.UpdateAsync(new(this));

        logger.LogTrace(LogTags.Other.AsEventId(), $"The task {Id} is finished.");

        if (exception != null)
        {
            progress.ReportProgress(0);
            throw exception;
        }
    }

    public string Serialize() => Id;

    public void SetStatus(TaskStatus status, string comment)
    {
        Status = status;
        var now = DateTime.UtcNow;

        _ = taskRepository.CreateAsync(new()
        {
            Comment = comment,
            Date = now,
            TaskId = Id,
            Status = Status
        });

        logger.LogTrace(LogTags.TaskChangeEvent.AsEventId(), $"The task {Id} status changed {status} - {comment}.");
    }

    public abstract void ProcessRelatedTasks(IEnumerable<IBackgroundTask> tasks);

    public bool IsSubjectSame(IBackgroundTask task)
    {
        if (EntityId == default || task.EntityId == default)
        {
            return false;
        }

        return EntityId.Equals(task.EntityId);
    }

    #endregion

    #region Protected Methods

    protected abstract Task OnProgressAsync(double progress);

    protected abstract Task InternalExecuteAsync();

    protected Guid GetMetadataIdFromTaskId(string taskId, TaskErrorCode error)
    {
        if (taskId.Contains("_v"))
        {
            taskId = taskId.Split('_')[0];
        }

        if (Guid.TryParse(taskId.Substring(taskId.IndexOf(':') + 1), out Guid metadataId))
        {
            return metadataId;
        }

        Error = error;

        throw new Exception(string.Format($"Invalid task Id {taskId}"));
    }

    #endregion
}
