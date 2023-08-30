namespace Brupper.AspNetCore.Exceptions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class DomainExceptionFactory
{
    public static DomainException CreateAlreadyExists(string errorMessage, string? clientMessage = null)
                => new EntityAlreadyExistsException(clientMessage ?? string.Empty, errorMessage);

    public static DomainException CreateNotFound(string errorMessage, string? clientMessage = null)
        => new EntityNotFoundException(clientMessage ?? string.Empty, errorMessage);

    public static DomainException CreateBadRequest(string message)
        => CreateBadRequest(message, message);

    public static DomainException CreateBadRequest(string clientMessage, string errorMessage)
        => new DomainException(DomainExceptionType.BadRequest, clientMessage, errorMessage);

    public static DomainException CreateOperationFailed(string clientMessage, string errorMessage)
        => new DomainException(DomainExceptionType.OperationFailed, clientMessage, errorMessage);
}
