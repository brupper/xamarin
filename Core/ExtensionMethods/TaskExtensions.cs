using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static Task<T> RetryOnFault<T>(this object fake, Func<Task<T>> functionToRetry, int maxTries, Func<Task> retryWhen = null)
        => RetryOnFault(functionToRetry, maxTries, retryWhen);

    public static async Task<T> RetryOnFault<T>(Func<Task<T>> functionToRetry, int maxTries, Func<Task> retryWhen = null)
    {
        for (var i = 0; i < maxTries; i++)
        {
            try { return await functionToRetry().ConfigureAwait(false); }
            catch { if (i == maxTries - 1) throw; }

            if (retryWhen != null)
            {
                await retryWhen().ConfigureAwait(false);
            }
        }

        return default;
    }

    public static Task RetryOnFault(this object fake, Func<Task> functionToRetry, int maxTries, Func<Task> retryWhen = null)
        => RetryOnFault(functionToRetry, maxTries, retryWhen);

    public static async Task RetryOnFault(Func<Task> functionToRetry, int maxTries, Func<Task> retryWhen = null)
    {
        for (var i = 0; i < maxTries; i++)
        {
            try
            {
                await functionToRetry().ConfigureAwait(false);
                break;
            }
            catch { if (i == maxTries - 1) throw; }

            if (retryWhen != null)
            {
                await retryWhen().ConfigureAwait(false);
            }
        }
    }

    /// <summary> This extension is wonderful. When you need to run a task safely, you can do it with this extension and let you write your try {} catch {} blocks. </summary>
    /// <example>
    /// var task = new Task(() => { Console.WriteLine("Hi!"); });
    /// await task.RunSafe(() => Console.WriteLine("An error occured!"));
    /// </example>
    /// <param name="task"></param>
    /// <param name="onError"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task RunSafe(this Task task, Func<Exception, Task> onError = null, CancellationToken token = default)
    {
        Exception exception = null;

        try
        {
            if (!token.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    task.Start();
                    task.Wait();
                });
            }
        }
        catch (TaskCanceledException) { Debug.WriteLine("Task Cancelled"); }
        catch (AggregateException e)
        {
            var ex = e.InnerException;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            exception = ex;
        }
        catch (Exception e)
        {
            exception = e;
        }
        finally
        {
            //AfterTaskRun?.Invoke(null, task);
        }

        if (exception != null)
        {
            Debug.WriteLine(exception);

            if (onError != null)
            {
                await onError(exception);
            }
        }
    }
}
