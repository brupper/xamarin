namespace Brupper.AspNetCore.Exceptions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityAlreadyExistsException : DomainException
{
    public EntityAlreadyExistsException(string clientMessage, string message) : base(DomainExceptionType.AlreadyExists, clientMessage, message) { }
}
