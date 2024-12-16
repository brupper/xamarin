namespace Brupper.Jobs;

public static class TaskProgressChangedEvent
{
    public static TaskProgressChangedEvent<TTask> With<TTask>(TTask task, int progress)
        where TTask : IBackgroundTask
        => new(task, progress);
}

public class TaskProgressChangedEvent<TTask>(TTask task, int progress) : MessagingEvent
    where TTask : IBackgroundTask
{
    public int Progress { get; } = progress;

    public TTask Task { get; } = task;
}