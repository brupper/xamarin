namespace MediatR.BackgroundService.BackgroundServices;

/// <summary>
/// A request for the long operation. This will be ideally executed in the background
/// </summary>
public record LongOperationRequest(string Source) : IRequest<Unit>;


public class LongOperationHandler(ILogger<LongOperationHandler> logger) : IRequestHandler<LongOperationRequest, Unit>
{
    private readonly ILogger<LongOperationHandler> logger = logger;

    public async Task<Unit> Handle(LongOperationRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Started long operation");

        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

            logger.LogInformation($"Long operation running from source: {request} is at {i}%");
        }

        return Unit.Value;
    }
}