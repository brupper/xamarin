using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Brupper.Jobs.Tasks.Fakes;

public class FakeLongRunningModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Delay { get; set; } = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
}

/// <inheritdoc/>
public class FakeLongRunningTask(
    IBackgroundTaskLogRepository uploadTaskLogLocalService,
    IBackgroundTaskRepository uploadTaskLocalService,
    ILogger<FakeLongRunningTask> logger)
    : AbstractTask(uploadTaskLogLocalService, uploadTaskLocalService, logger)
    , IBackgroundTask<FakeLongRunningModel>
{
    #region Fields

    /// <summary> The task id of the download task. </summary>
    public const string TaskIdTemplate = "FakeLongRunningTask:{0}";

    public FakeLongRunningModel Entity { get; protected set; }

    #endregion

    #region AbstractTask implementation

    protected override async Task InternalExecuteAsync()
    {
        try
        {
            var totalEllapsed = 0d;
            Timer timer = new();
            timer.AutoReset = true;
            timer.Interval = 100;
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                totalEllapsed += timer.Interval;
                var progressValue = (100 * totalEllapsed) / Entity.Delay;
                _ = OnProgressAsync(progressValue);
            };
            timer.Start();

            progress.ReportProgress(50);
            await Task.Delay(Entity.Delay);
            timer.Stop();
            progress.ReportProgress(75);

            SetStatus(TaskStatus.Completed, string.Format($"File finished with id: {Id}"));
        }
        catch (Exception e)
        {
            SetStatus(TaskStatus.Error, string.Format($"Error when processing {Entity}, Error {e}"));
            throw;
        }
    }

    public override void ProcessRelatedTasks(IEnumerable<IBackgroundTask> tasks)
    {
        var tmpTaskLIst = tasks?.ToList() ?? new List<IBackgroundTask>();
        /*
        if (tmpTaskLIst.Any(t => t is FakeLongRunningDeleteTask deleteTask && deleteTask.Entity == Entity))
        {
            // upload task for the same uuid Entity cannot run after a delete task,
            // check for existing delete task for the same Entity and abort itself 
            Abort();
        }
        // */
    }

    protected override Task OnProgressAsync(double progress)
    {
        StrongReferenceMessenger.Default?.Send<TaskProgressChangedEvent<IBackgroundTask>>(TaskProgressChangedEvent.With((IBackgroundTask)this, (int)Math.Round(progress)));

        base.logger.LogTrace(LogTags.Other.AsEventId(), $"[{DateTime.Now.TimeOfDay}] The {Id} {Entity} progress report: {progress}");

        return Task.CompletedTask;
    }

    #endregion

    #region ITask<T> implementation

    public async Task InitAsync(FakeLongRunningModel entity)
    {
        Guard.IsNotNull(entity, nameof(entity));

        this.Entity = entity;
        Id = GetTaskId(entity);
        // Audit = await repository.GetLocalAsync(someUuid);

        await Task.CompletedTask;
    }

    #endregion

    protected virtual string GetTaskId(FakeLongRunningModel entity) => GetId(entity);

    /// <summary> Returns the id of the task. </summary>
    public static string GetId(FakeLongRunningModel entity) => string.Format(CultureInfo.InvariantCulture, TaskIdTemplate, entity.Id);

    public override async Task DeserializeAsync(string info)
    {
        var id = GetMetadataIdFromTaskId(info, TaskErrorCode.InvalidTaskId);

        /*
        var entity = await repository.GetLocalAsync(id);
        if (entity == null || entity.IsDeleted)
        {
            Error = UploadError.DeserializationError;
            var exceptionMessage = string.Format($"Cannot find Task by id: {id}");

            try
            {
                transferManager.Abort(this);
            }
            catch
            {
                throw new Exception(exceptionMessage);
            }

            throw new Exception(exceptionMessage);
        }

        await InitAsync(entity);
        // */

        await InitAsync(new FakeLongRunningModel { Id = id.ToString(), });
    }
}
