using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IPriorityQueue"/> implementation. </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Brupper.Jobs.Implementations.PriorityQueue"/> class.
/// </remarks>
/// <param name="uploadTaskLocalService">The upload task local service.</param>
public class PriorityQueue(
    IBackgroundTaskRepository uploadTaskLocalService,
    IServiceProvider serviceProvider,
    ILogger/*<PriorityQueue>*/ logger)
    : IPriorityQueue
{
    #region Private Properties

    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly ILogger logger = logger;
    private readonly IBackgroundTaskRepository uploadTaskLocalService = uploadTaskLocalService;

    private readonly Dictionary<string, IBackgroundTask> taskLookupById = new();
    private readonly LinkedList<IBackgroundTask> tasks = new();

    #endregion

    #region IPriorityQueue implementation

    public int Count => tasks.Count;

    public Task AddAfter(IBackgroundTask task, IBackgroundTask newTask)
    {
        var taskNode = tasks.Find(task);
        if (taskNode == null)
        {
            //Logger.Current.TrackTrace(LogTag.Critical, LoggingLabels.TaskRetrievalStrategyTaskIsNullAndSkipAdd, new Metadata
            //{
            //    { $"Param-{nameof(task)}" ,task?.Id },
            //    { $"Param-{nameof(newTask)}", newTask?.Id },
            //});
            return Task.CompletedTask;
        }

        return Add(newTask, t => tasks.AddAfter(taskNode, t));
    }

    public Task AddBefore(IBackgroundTask task, IBackgroundTask newTask)
    {
        var taskNode = tasks.Find(task);
        if (taskNode == null)
        {
            //Logger.Current.TrackTrace(LogTag.Critical, LoggingLabels.TaskRetrievalStrategyTaskIsNullAndSkipAdd, new Metadata
            //{
            //    { $"Param-{nameof(task)}" ,task?.Id },
            //    { $"Param-{nameof(newTask)}", newTask?.Id },
            //});
            return Task.CompletedTask;
        }

        return Add(newTask, t => tasks.AddBefore(taskNode, t));
    }

    public bool Contains(IBackgroundTask task)
    {
        if (task == null) return false;

        return taskLookupById.ContainsKey(task.Id);
    }

    public async Task<IBackgroundTask> Dequeue()
    {
        var next = PeekNext();

        if (next != null) await Remove(next);

        return next;
    }

    public Task Enqueue(IBackgroundTask task)
    {
        return Add(task, t => tasks.AddLast(t));
    }

    public Task EnqueueOnTop(IBackgroundTask task)
    {
        return Add(task, t => tasks.AddFirst(t));
    }

    public IBackgroundTask Find(string id)
    {
        if (!taskLookupById.ContainsKey(id))
        {
            //Logger.Current.TrackTrace(LogTag.Critical, LoggingLabels.TaskRetrievalStrategyTaskIsNull, new Metadata("taskId", id));
            return null;
        }

        return taskLookupById[id];
    }

    public IEnumerator<IBackgroundTask> GetEnumerator()
    {
        return tasks.GetEnumerator();
    }

    public IBackgroundTask PeekLast()
    {
        if (tasks.Count == 0) return null;

        return tasks.Last.Value;
    }

    public IBackgroundTask PeekNext()
    {
        if (tasks.Count == 0) return null;

        return tasks.First.Value;
    }

    public async Task<IBackgroundTask> Remove(IBackgroundTask task)
    {
        if (task != null)
        {
            if (!taskLookupById.ContainsKey(task.Id))
            {
                return task;
            }

            tasks.Remove(task);
            taskLookupById.Remove(task.Id);

            await uploadTaskLocalService.DeleteAsync(new BackgroundTask(task));
        }

        return task;
    }

    public async Task Restore()
    {
        taskLookupById.Clear();
        tasks.Clear();

        var uploadTasks = await uploadTaskLocalService.GetTasksToPerformAsync();

        foreach (var uploadTask in uploadTasks)
        {
            var task = await RestoreTask(uploadTask);
            if (string.IsNullOrEmpty(task?.Id))
            {
                await uploadTaskLocalService.DeleteAsync(uploadTask);
                continue;
            }

            await Enqueue(task);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return tasks.GetEnumerator();
    }

    public IEnumerable<IBackgroundTask> GetItems()
    {
        return tasks.ToList();
    }

    #endregion

    #region Private Methods

    private async Task Add(IBackgroundTask task, Func<IBackgroundTask, LinkedListNode<IBackgroundTask>> addAction)
    {
        if (task == null)
        {
            return;
        }

        var newNode = addAction(task);

        SetRank(newNode);

        var taskToCreate = new BackgroundTask(newNode.Value);
        var taskExist = await uploadTaskLocalService.GetAsync(newNode.Value.Id);
        if (taskExist == null)
        {
            await uploadTaskLocalService.CreateAsync(taskToCreate);
        }

        if (!taskLookupById.ContainsKey(task.Id))
        {
            taskLookupById.Add(task.Id, task);
        }
    }

    private void SetRank(LinkedListNode<IBackgroundTask> node)
    {
        var nextRank = int.MaxValue;
        var prevRank = int.MinValue;
        var newRank = 0;

        if (node.Next != null) nextRank = node.Next.Value.Rank;
        if (node.Previous != null) prevRank = node.Previous.Value.Rank;

        if (node.Previous == null)
        {
            if (node.Next != null) newRank = Math.Min(0, nextRank - 1);
        }
        else
        {
            if (node.Next != null) newRank = nextRank;
            else newRank = prevRank + 1;
        }

        node.Value.Rank = newRank;
    }

    private async Task<IBackgroundTask> RestoreTask(BackgroundTask uploadTask)
    {
        IBackgroundTask task = null;

        try
        {
            task = (IBackgroundTask)ActivatorUtilities.CreateInstance(serviceProvider, uploadTask.TaskType);
            task.Rank = uploadTask.Rank;
            task.CreationDate = uploadTask.CreatedAt.ToUniversalTime();
            task.LastModificationDate = uploadTask.ModifiedAt.ToUniversalTime();
            await task.DeserializeAsync(uploadTask.TaskId);

            task.SetStatus(uploadTask.Status, "Restoring task");
        }
        catch (Exception e)
        {
            logger.LogError($"{e}");
        }

        return task;
    }

    #endregion
}
