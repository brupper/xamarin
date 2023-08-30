namespace Brupper.AspNetCore.Exceptions;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class DomainException : Exception
{
    public DomainExceptionType Type { get; private set; }

    public string ClientMessage { get; private set; }

    public DomainException(DomainExceptionType type, string clientMessage, string message) : base(message)
    {
        Type = type;
        ClientMessage = clientMessage;
    }
}
