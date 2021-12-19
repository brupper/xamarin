using System;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public class DiagnosticsException : Exception
    {
        public DiagnosticsException() : base() { }

        public DiagnosticsException(string exceptionMessage) : base(exceptionMessage) { }

        public DiagnosticsException(string exceptionMessage, Exception innerException) : base(exceptionMessage, innerException) { }
    }
}