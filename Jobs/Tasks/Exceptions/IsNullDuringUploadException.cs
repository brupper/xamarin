using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Brupper.Jobs.Tasks;


[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
[Serializable]
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class IsNullDuringUploadException : Exception
{
    public IsNullDuringUploadException() { }

    public IsNullDuringUploadException(string message) : base(message) { }

    public IsNullDuringUploadException(string message, Exception innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051
    protected IsNullDuringUploadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning disable SYSLIB0051
}
