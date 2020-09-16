using System;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static Task<T> RetryOnFault<T>(this object fake, Func<Task<T>> function, int maxTries, Func<Task> retryWhen = null)
        => RetryOnFault(function, maxTries, retryWhen);

    public static async Task<T> RetryOnFault<T>(Func<Task<T>> function, int maxTries, Func<Task> retryWhen = null)
    {
        for (var i = 0; i < maxTries; i++)
        {
            try { return await function().ConfigureAwait(false); }
            catch { if (i == maxTries - 1) throw; }

            if (retryWhen != null)
            {
                await retryWhen().ConfigureAwait(false);
            }
        }

        return default;
    }

    public static Task RetryOnFault(this object fake, Func<Task> function, int maxTries, Func<Task> retryWhen = null)
        => RetryOnFault(function, maxTries, retryWhen);

    public static async Task RetryOnFault(Func<Task> function, int maxTries, Func<Task> retryWhen = null)
    {
        for (var i = 0; i < maxTries; i++)
        {
            try
            {
                await function().ConfigureAwait(false);
                break;
            }
            catch { if (i == maxTries - 1) throw; }

            if (retryWhen != null)
            {
                await retryWhen().ConfigureAwait(false);
            }
        }
    }
}
