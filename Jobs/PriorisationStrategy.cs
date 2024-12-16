using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IPriorisationStrategy"/> implementation. </summary>
public class PriorisationStrategy : IPriorisationStrategy
{
    #region Private Fields

    private object lockObject = new();

    private IPriorityQueue dataSource;

    #endregion

    #region IPriorisationStrategy implementation

    public IPriorityQueue DataSource
    {
        get
        {
            lock (lockObject)
            {
                return dataSource;
            }
        }
        set
        {
            lock (lockObject)
            {
                dataSource = value;
            }
        }
    }

    public Task EnqueueAsync(IBackgroundTask newTask)
    {
        IEnumerable<IBackgroundTask> relatedTask = newTask.GetRelatedTasks(DataSource);

        newTask.ProcessRelatedTasks(relatedTask);

        RemoveAbortedTasksFromDataSource(relatedTask);

        return HandleNewTask(newTask, relatedTask);
    }

    private void RemoveAbortedTasksFromDataSource(IEnumerable<IBackgroundTask> relatedTask)
    {
        foreach (var abortedTask in relatedTask.Where(t => t.IsAborted))
        {
            DataSource.Remove(abortedTask);
        }
    }

    public async Task HandleErrorAsync(IBackgroundTask errorTask)
    {
        await DataSource.Remove(errorTask);
        await DataSource.Enqueue(errorTask);
    }

    #endregion

    #region Private Methods

    private Task HandleNewTask(IBackgroundTask newTask, IEnumerable<IBackgroundTask> relatedTask)
    {
        if (newTask.IsAborted)
        {
            return Task.FromResult(true);
        }

        /*
        // UploadTask should add to the top of the queue.
        if (newTask.GetType() == typeof(AuditCreateTask))
        {
            //Add after the last close task which not belongs to the same task
            var closeTask = DataSource.LastOrDefault(x => x.GetType() == typeof(AuditCloseTask) && newTask.Audit != x.Audit);
            if (closeTask != null)
            {
                return DataSource.AddAfter(closeTask, newTask);
            }
            else
            {
                return DataSource.EnqueueOnTop(newTask);
            }
        }

        if (newTask.GetType() == typeof(PhotoUploadTask) || newTask.GetType() == typeof(SurveyUploadTask))
        {
            var closeTask = DataSource.FirstOrDefault(x => x.GetType() == typeof(AuditCloseTask) && newTask.Audit == x.Audit);
            if (closeTask != null)
            {
                return DataSource.AddBefore(closeTask, newTask);
            }

            return DataSource.Enqueue(newTask);
        }
        else if (newTask.GetType() == typeof(AuditCloseTask))
        {
            return DataSource.Enqueue(newTask);
        }

        // UpdateTask should add at the end of the queue, because it is possible that there is an UploadTask already in the queue.
        else if (newTask.GetType() == typeof(AuditUpdateTask) || newTask.GetType() == typeof(PhotoUpdateTask) || newTask.GetType() == typeof(SurveyUpdateTask))
        {
            var updateTask = DataSource.LastOrDefault(x => x is PhotoUpdateTask);
            var closeTask = DataSource.LastOrDefault(x => x.GetType() == typeof(AuditCloseTask));
            if (closeTask != null)
            {
                return DataSource.AddBefore(closeTask, newTask);
            }
            else if (newTask is AuditUpdateTask)
            {
                return DataSource.AddBefore(updateTask, newTask);
            }
            else
            {
                return DataSource.Enqueue(newTask);
            }
        }
        else
        */
        {
            return DataSource.Enqueue(newTask);
        }
    }

    #endregion
}
