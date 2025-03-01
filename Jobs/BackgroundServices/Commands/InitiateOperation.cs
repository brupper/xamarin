namespace MediatR.BackgroundService.BackgroundServices;

public record InitiateOperationRequest() : IRequest<InitiateOperationResponse>;

public record InitiateOperationResponse(string Value = default!);


public class InitiateOperationHanlder(IMediatorBackground mediatorBackground) : IRequestHandler<InitiateOperationRequest, InitiateOperationResponse>
{
    private readonly IMediatorBackground mediatorBackground = mediatorBackground;

    public async Task<InitiateOperationResponse> Handle(InitiateOperationRequest request, CancellationToken cancellationToken)
    {
        LongOperationRequest backgroundRequest = new("Handler");

        await mediatorBackground.Send(backgroundRequest);

        var response = new InitiateOperationResponse
        {
            Value = "Initiation from handler has been successful."
        };

        return response;
    }
}
