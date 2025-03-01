using MediatR.BackgroundService;
using MediatR.BackgroundService.BackgroundServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class MediatorBackgroundConfiguration
{
    public static IServiceCollection AddBackgroundQueueServices(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddSingleton<IMediatorBackground, MediatorBackground>();
        return services;
    }
}
