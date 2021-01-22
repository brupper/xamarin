using System;

namespace Brupper.Data.RemoteServices.Exceptions
{
    public class ApiException : Exception
    {
        public int RawCode { get; set; }

        public ApiException(int code, string message, Exception innerException)
            : base(message, innerException)
        {
            RawCode = code;
        }

        public ApiException(string message, Exception innerException)
            : base(message, innerException) { }

        public ApiException(int code, string message)
            : base(message)
        {
            RawCode = code;
        }

        public ApiException(string message)
            : base(message) { }
    }
}
