namespace Brupper.AspNetCore.Exceptions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string clientMessage, string message) : base(DomainExceptionType.NotFound, clientMessage, message) { }
}
