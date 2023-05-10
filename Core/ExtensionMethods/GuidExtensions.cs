using System;

namespace Brupper
{
    public static class GuidExtensions
    {
        public static string ToShortString(this Guid guid) => guid.ToString().ToLower().Replace("-", "");
    }
}
