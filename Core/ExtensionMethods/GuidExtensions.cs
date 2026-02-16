using System;

namespace Brupper;

public static class GuidExtensions
{
    public static string Substring(this Guid guid, int length) => guid.ToShortString().Substring(0, length);

    public static string ToShortString(this Guid guid) => guid.ToString().ToLower().Replace("-", "");
}
