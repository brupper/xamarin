using System.Collections.Generic;
using System.Linq;

namespace Brupper.Jobs;

public static class UploadTaskExtensions
{
    /// <summary> Gets the related tasks. </summary>
    /// <param name="currentTask">The current task.</param>
    /// <param name="tasks">The list of tasks in queue.</param>
    /// <returns>The list of related tasks.</returns>
    public static IEnumerable<IBackgroundTask> GetRelatedTasks(this IBackgroundTask currentTask, IEnumerable<IBackgroundTask> tasks)
    {
        IEnumerator<IBackgroundTask> tasksInQueue = tasks.ToList().GetEnumerator();
        List<IBackgroundTask> relatedTask = new List<IBackgroundTask>();

        while (tasksInQueue.MoveNext())
        {
            var task = tasksInQueue.Current;

            if (!currentTask.Equals(task) && currentTask.IsSubjectSame(task))
            {
                relatedTask.Add(task);
            }
        }

        return relatedTask;
    }
}
