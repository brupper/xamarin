using System;
using System.Collections.Generic;
using System.Linq;

public static class ParseHelper
{
    public static readonly string[] RecordDelimiter = new[] { "##" };


    public static string ToDelimitedString(this IEnumerable<string> collection)
    {
        if (collection != null)
        {
            return string.Join(RecordDelimiter[0], collection);
        }

        return null;
    }

    public static IEnumerable<string> FromDelimitedString(this string joinedCollection)
    {
        if (joinedCollection != null)
        {
            return joinedCollection.Split(RecordDelimiter, StringSplitOptions.RemoveEmptyEntries);
        }

        return Enumerable.Empty<string>();
    }
}
