using Microsoft.Extensions.DependencyInjection;

namespace Brupper.Jobs;

public static class Module
{
    public static IServiceCollection AddBackgroundJobAbstractions(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskLogRepository, BackgroundTaskLogRepository>();
        services.AddSingleton<IBackgroundTaskRepository, BackgroundTaskRepository>();

        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        services.AddSingleton<IBackgroundTaskManager, BackgroundTaskManager>();

        return services;
    }
}
