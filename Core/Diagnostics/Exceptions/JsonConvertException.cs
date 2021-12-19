using System;

namespace Brupper.Diagnostics
{
    public class JsonConvertException : Exception
    {
        public string Json { get; }

        public JsonConvertException(string exceptionMessage, Exception innerException, string json)
            : base(exceptionMessage, innerException)
        {
            Json = json;
        }

        public JsonConvertException() : base() { }

        public JsonConvertException(string message) : base(message) { }

        public JsonConvertException(string message, Exception innerException) : base(message, innerException) { }
    }
}