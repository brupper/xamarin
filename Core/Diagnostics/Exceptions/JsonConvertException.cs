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
    }
}