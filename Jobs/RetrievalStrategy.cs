using System;
using System.Threading.Tasks;

namespace Brupper.Jobs;

/// <summary> <see cref="Brupper.Jobs.IRetrievalStrategy"/> implementation. </summary>
public class RetrievalStrategy : IRetrievalStrategy
{
    #region Constants

    private const double RetryTimeInMin = 0.5;

    #endregion

    #region Private Fields

    private object lockObject = new object();

    private IPriorityQueue dataSource;

    #endregion

    #region ITaskRetrievalStrategy implementation

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

    public Task<IBackgroundTask> GetNext()
    {
        IBackgroundTask result = null;

        foreach (var item in DataSource.GetItems())
        {
            if (item == null)
            {
                // Logger.Current.TrackTrace(LogTag.Critical, LoggingLabels.TaskRetrievalStrategyTaskIsNull);
                continue;
            }

            if (CanExecute(item))
            {
                result = item;
                break;
            }
        }

        return result.AsTask();
    }

    #endregion

    #region Private Methods

    private bool CanExecute(IBackgroundTask item)
    {
        var canRun = Math.Abs((item.LastModificationDate.ToUniversalTime() - DateTime.UtcNow).TotalMinutes) >= RetryTimeInMin;

        return item.Status == TaskStatus.Pending || (item.Status == TaskStatus.Error && canRun);
    }

    #endregion
}
