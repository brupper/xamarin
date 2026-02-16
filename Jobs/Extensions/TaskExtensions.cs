using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Brupper;

/// <summary> Add extensions for <see cref="System.Threading.Tasks.Task"/>. </summary>
internal static class TaskExtensions
{
    /// <summary> Transforms the provided value into the result of a completed <see cref="System.Threading.Tasks.Task"/>. </summary>
    /// <typeparam name="T">The type of the value to return.</typeparam>
    /// <param name="target">The value that will be wrapped in a completed <see cref="System.Threading.Tasks.Task"/>.</param>
    /// <returns>A completed instance of <see cref="System.Threading.Tasks.Task"/> that gives the provided value as its result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> AsTask<T>(this T target)
    {
        return Task.FromResult(target);
    }

    /// <summary>
    /// SendEmailAsync().FireAndForget(errorHandler => Console.WriteLine(errorHandler.Message));
    /// 
    /// Sometimes you want to fire and forget a task. This means that you want to start a task 
    /// but you don't want to wait for it to finish. This is useful when you want to start a 
    /// task but you don't care about the result (non-critical tasks). For example when you want
    /// to start a task that sends an email. You don't want to wait for the email to be sent before 
    /// you can continue with your code. So you can use the FireAndForget extension method to start
    /// the task and forget about it. Optionally you can pass an error handler to the method. This 
    /// error handler will be called when the task throws an exception.
    /// </summary>
    public static void FireAndForget(this Task task, Action<Exception> errorHandler = default)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted && errorHandler != null)
            {
                errorHandler(t.Exception);
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }

    public static void Forget(this Task task)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task);
        }

        async static Task ForgetAwaited(Task task)
        {
            await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }
    }

    /// <summary>
    /// var result = await (() => GetResultAsync()).Retry(3, TimeSpan.FromSeconds(1));
    /// 
    /// If you want to retry a task a specific number of times, you can use the Retry extension method. 
    /// This method will retry the task until it succeeds or the maximum number of retries is reached. 
    /// You can pass a delay between retries. This delay will be used between each retry.
    /// </summary>
    public static async Task<TResult> Retry<TResult>(this Func<Task<TResult>> taskFactory, int maxRetries, TimeSpan delay)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await taskFactory().ConfigureAwait(false);
            }
            catch
            {
                if (i == maxRetries - 1)
                    throw;
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }

        return default(TResult); // Should not be reached
    }

    /// <summary>
    /// await GetResultAsync().OnFailure(ex => Console.WriteLine(ex.Message));
    /// </summary>
    public static async Task OnFailure(this Task task, Action<Exception> onFailure)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            onFailure(ex);
        }
    }

    /// <summary>
    /// await GetResultAsync().WithTimeout(TimeSpan.FromSeconds(1));
    /// 
    /// Sometimes you want to set a timeout for a task. This is useful when you want to 
    /// prevent a task from running for too long. You can use the Timeout extension method 
    /// to set a timeout for a task. If the task takes longer than the timeout the task will
    /// be cancelled.
    /// </summary>
    public static async Task WithTimeout(this Task task, TimeSpan timeout)
    {
        var delayTask = Task.Delay(timeout);
        var completedTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
        if (completedTask == delayTask)
            throw new TimeoutException();

        await task;
    }

    /// <summary>
    /// var result = await GetResultAsync().Fallback("fallback");
    /// 
    /// Sometimes you want to use a fallback value when a task fails.
    /// You can use the Fallback extension method to use a fallback value when a task fails.
    /// </summary>
    public static async Task<TResult> Fallback<TResult>(this Task<TResult> task, TResult fallbackValue)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        catch
        {
            return fallbackValue;
        }
    }
}
