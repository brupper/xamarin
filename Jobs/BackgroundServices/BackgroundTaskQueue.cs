using System.Threading.Channels;

namespace MediatR.BackgroundService.BackgroundServices;

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services#queued-background-tasks
internal class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<IServiceProvider, CancellationToken, ValueTask>> queue;
    private readonly ILogger<BackgroundTaskQueue> logger;

    public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
    {
        this.logger = logger;

        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.            
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        var queueCapacity = 10; //refer to docs on setting the capacity
        var options = new BoundedChannelOptions(queueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        queue = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, ValueTask>>(options);
    }

    public async ValueTask<Func<IServiceProvider, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await queue.Reader.ReadAsync(cancellationToken);

        logger.LogInformation("Item {workItemName} has been read and will be dequeued from background queue.", nameof(workItem));

        return workItem;
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, ValueTask> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        logger.LogInformation("Preparing to write item {workItemName} to background queue.", nameof(workItem));

        await queue.Writer.WriteAsync(workItem);

        logger.LogInformation("Item {workItemName} has been written to background queue and will be executed.",
            nameof(workItem));
    }
}
