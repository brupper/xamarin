using Brupper.Jobs;

namespace Microsoft.Extensions.DependencyInjection;

public static class Module
{
    public static IServiceCollection AddBackgroundJobAbstractions(this IServiceCollection services)
    {
        services.AddBackgroundQueueServices();

        services.AddSingleton<IBackgroundTaskLogRepository, BackgroundTaskLogRepository>();
        services.AddSingleton<IBackgroundTaskRepository, BackgroundTaskRepository>();

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services.AddSingleton<IBackgroundTaskManager, BackgroundTaskManager>();

        return services;
    }
}
