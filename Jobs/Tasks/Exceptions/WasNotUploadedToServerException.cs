using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Brupper.Jobs.Tasks;


[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Serializable]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class WasNotUploadedToServerException : Exception
{
    public WasNotUploadedToServerException() { }

    public WasNotUploadedToServerException(string message) : base(message) { }

    public WasNotUploadedToServerException(string message, Exception innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051
    protected WasNotUploadedToServerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning disable SYSLIB0051
}
