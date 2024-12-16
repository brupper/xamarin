using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Brupper.Jobs.FileTransfer.Azure.Exceptions
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [Serializable]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AccessTokenRetrieveException : Exception
    {
        public AccessTokenRetrieveException() { }

        public AccessTokenRetrieveException(string message) : base(message) { }

        public AccessTokenRetrieveException(string message, Exception innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051
        protected AccessTokenRetrieveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning disable SYSLIB0051

        public string Json { get; set; }

        private string GetDebuggerDisplay() => ToString();
    }
}
