using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Brupper;

public static class CollectionExtensions
{
    public static bool IsSequential<T>(this IEnumerable<T> source, Func<T, int> indexGetter, int startIndex)
    {
        return source.Select(indexGetter).IsSequential(startIndex);
    }

    public static bool IsSequential(this IEnumerable<int> source, int startIndex)
    {
        return source.Select((value, index) => (value, index)).All(x => x.value == startIndex + x.index);
    }

    [Pure]
    public static bool IsEmptyOrNull<T>(this IReadOnlyCollection<T> source)
    {
        return source is null || source.Count == 0;
    }
}