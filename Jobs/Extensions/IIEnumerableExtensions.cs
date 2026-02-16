using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

internal static class IIEnumerableExtensions
{
    /// <summary> Returns the number of elements in a sequence. </summary>
    public static int Count(this IEnumerable source)
    {
        var col = source as ICollection;
        if (col != null)
            return col.Count;

        int c = 0;
        var e = source.GetEnumerator();
        while (e.MoveNext())
            c++;

        return c;
    }

    public static bool Empty<T>(this IEnumerable<T> source) => !(source?.Any() ?? false);

    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
    {
        if (collection == null || collection.Count == 0)
        {
            return;
        }

        var sortableList = new List<T>(collection);
        if (comparison == null)
        {
            sortableList.Sort();
        }
        else
        {
            sortableList.Sort(comparison);
        }

        for (var i = 0; i < sortableList.Count; i++)
        {
            var oldIndex = collection.IndexOf(sortableList[i]);
            var newIndex = i;
            if (oldIndex != -1 && oldIndex != newIndex)
            {
                collection.Move(oldIndex, newIndex);
            }
        }
    }
}
