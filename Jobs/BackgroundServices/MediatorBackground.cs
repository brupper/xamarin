using MediatR.BackgroundService.BackgroundServices;

namespace MediatR.BackgroundService;

internal class MediatorBackground(
    IBackgroundTaskQueue backgroundTaskQueue,
    IServiceScopeFactory serviceScopeFactory)
    : IMediatorBackground
{
    private readonly IBackgroundTaskQueue backgroundTaskQueue = backgroundTaskQueue;
    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    public async ValueTask Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        await backgroundTaskQueue.QueueBackgroundWorkItemAsync(async (sp, stoppingToken) =>
        {
            using var scope = serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            await mediator.Send(request, cancellationToken);
        });
    }
}
