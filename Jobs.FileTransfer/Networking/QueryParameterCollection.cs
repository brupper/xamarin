using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brupper.Jobs.FileTransfer;

/// <summary> Represents a collection of parameters to be used in HTTP operations. </summary>
public class QueryParameterCollection : IEnumerable, IEnumerable<KeyValuePair<string, string>>
{
    private const char QueryStringStartCharacter = '?';

    private static readonly string[] QueryStringSeparator = new string[] { "&" };

    private static readonly string[] NameValuePairSeparator = new string[] { "=" };

    private List<string> values;

    private List<string> keys;

    private bool escapeStrings;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
    /// </summary>
    /// <remarks>Values are always escaped.</remarks>
    public QueryParameterCollection()
        : this(true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
    /// </summary>
    /// <param name="escapeStrings">if set to <c>true</c> values are escaped when the query string is created.</param>
    public QueryParameterCollection(bool escapeStrings)
    {
        values = new List<string>();
        keys = new List<string>();
        this.escapeStrings = escapeStrings;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class populating it
    /// with values from an existing collection.
    /// </summary>
    /// <param name="baseCollection">The base collection with which the new instance will be initialized.</param>
    /// <remarks>Values will be escaped if the base collection escapes them too.</remarks>
    public QueryParameterCollection(QueryParameterCollection baseCollection)
        : this(baseCollection, baseCollection.escapeStrings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class populating it
    /// with values from an existing collection.
    /// </summary>
    /// <param name="baseCollection">The base collection with which the new instance will be initialized.</param>
    /// <param name="escapeStrings">if set to <c>true</c> values are escaped when the query string is created.</param>
    public QueryParameterCollection(QueryParameterCollection baseCollection, bool escapeStrings)
        : this(escapeStrings)
    {
        Merge(baseCollection);
    }

    /// <summary>
    /// Creates an instance of <see cref="QueryParameterCollection"/> from a URI that contains a query string.
    /// </summary>
    /// <param name="uri">The URI that will be parsed for its query string.</param>
    /// <returns>An instance of <see cref="QueryParameterCollection"/> initialized with values parsed from
    /// the provided URI.</returns>
    public static QueryParameterCollection FromUri(Uri uri)
    {
        var result = new QueryParameterCollection();

        if (uri.IsAbsoluteUri)
        {
            var query = uri.Query;

            if (query.IndexOf(QueryStringStartCharacter) == 0)
            {
                query = query.Substring(1);
            }

            result.SetValues(query);
        }
        else
        {
            int queryStringStartIndex = -1;

            if ((queryStringStartIndex = uri.OriginalString.IndexOf(QueryStringStartCharacter)) >= 0)
            {
                result.SetValues(uri.OriginalString.Substring(queryStringStartIndex + 1));
            }
        }

        return result;
    }

    /// <summary>
    /// Parses the specified query string and creates an instance of <see cref="QueryParameterCollection"/>.
    /// </summary>
    /// <param name="queryString">The query string to parse.</param>
    /// <returns>An instance of <see cref="QueryParameterCollection"/> initialized with values parsed from
    /// the provided query string.</returns>
    public static QueryParameterCollection Parse(string queryString)
    {
        var result = new QueryParameterCollection();

        result.SetValues(queryString);

        return result;
    }

    /// <summary>
    /// Merges the specified base collection into the current instance.
    /// </summary>
    /// <param name="baseCollection">The collection that will be merged into the current instance.</param>
    public void Merge(QueryParameterCollection baseCollection)
    {
        if (baseCollection != null)
        {
            for (int i = 0; i < baseCollection.keys.Count; i++)
            {
                keys.Add(baseCollection.keys[i]);
                values.Add(baseCollection.values[i]);
            }
        }
    }

    /// <summary>
    /// Merges the specified dictionary into the current instance.
    /// </summary>
    /// <param name="dictionary">The collection that will be merged into the current instance.</param>
    public void Merge(Dictionary<string, string> dictionary)
    {
        if (dictionary != null)
        {
            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }
    }

    /// <summary>
    /// Determines whether the specified key exists in the current collection.
    /// </summary>
    /// <param name="key">The key to locate in the current collection.</param>
    /// <returns><c>true</c> if the specified key is present in the current collection.</returns>
    public bool ContainsKey(string key)
    {
        return keys.Contains(key);
    }

    public void ChangeKeyValue<T>(string key, T newValue)
    {
        if (ContainsKey(key))
        {
            var i = keys.IndexOf(key);
            values[i] = newValue.ToString();
        }
    }

    /// <summary>
    /// Gets the string value corresponding to a given key.
    /// </summary>
    /// <param name="key">The key that identifies the requested value.</param>
    /// <returns>The value identified by the specified key or <c>null</c> if the key is not present in the collection.</returns>
    public string GetString(string key)
    {
        var i = keys.IndexOf(key);

        return (i >= 0) ? values[i] : null;
    }

    /// <summary>
    /// Gets the collection of values associated to a given key.
    /// </summary>
    /// <param name="key">The key that identifies the requested values.</param>
    /// <returns>A collection of values identified by the specified key or an empty enumeration if the key is not present in the collection.</returns>
    public IEnumerable<string> GetCollection(string key)
    {
        var enumKey = MakeCollectionKey(key);

        var indexes = keys.Select((k, i) => new { i, k }).Where(t => t.k.Equals(enumKey)).Select(t => t.i);

        return values.Select((v, i) => new { i, v }).Where(t => indexes.Contains(t.i)).Select(t => t.v).AsEnumerable();
    }

    /// <summary>
    /// Adds the specified key and value to the collection.
    /// </summary>
    /// <param name="tuple">The pair of key and value to be added to the collection.</param>
    public void Add(KeyValuePair<string, string> tuple)
    {
        Add(tuple.Key, tuple.Value);
    }

    /// <summary>
    /// Adds the specified key and value to the collection.
    /// </summary>
    /// <typeparam name="T">The type of the value to be added.</typeparam>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    /// <remarks>
    /// <para>If the specified value can be cast to a <see cref="System.String" />, it will be
    /// added directly into the collection.</para>
    /// <para>If the specified value is an <see cref="T:System.Collections.Generic.IEnumerable{string}" />, then
    /// the values will be added individually, but they will be collectively assigned to the same key.</para>
    /// <para>If the specified value implements <see cref="T:IQueryConvertible"/>, the collection resulting of the 
    /// conversion will be merged with the current collection.</para>
    /// </remarks>
    public void Add<T>(string key, T value)
    {
        string stringValue = value as string;

        if (stringValue != null)
        {
            AddInternal(key, stringValue);
            return;
        }

        IEnumerable<string> enumValue = value as IEnumerable<string>;

        if (enumValue != null)
        {
            var enumKey = MakeCollectionKey(key);

            foreach (var svalue in enumValue)
            {
                AddInternal(enumKey, svalue);
            }

            return;
        }

        // TODO: IQueryConvertible 

        AddInternal(key, value.ToString());
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        for (int i = 0; i < keys.Count; i++)
        {
            builder.AppendFormat("{0}{2}{1}{3}", keys[i], values[i], NameValuePairSeparator[0], QueryStringSeparator[0]);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            yield return new KeyValuePair<string, string>(keys[i], values[i]);
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static string MakeCollectionKey(string key)
    {
        return string.Format("{0}[]", key);
    }

    private void AddInternal(string key, string value)
    {
        keys.Add(key);
        values.Add(escapeStrings ? Uri.EscapeDataString(value) : value);
    }

    private void SetValues(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return;
        }

        var nameValuePairs = from nvp in queryString.Split(QueryStringSeparator, StringSplitOptions.RemoveEmptyEntries)
                             let tokens = nvp.Split(NameValuePairSeparator, StringSplitOptions.RemoveEmptyEntries)
                             let hasValue = tokens.Length == 2
                             select new Tuple<string, string>(tokens[0], hasValue ? Uri.UnescapeDataString(tokens[1]) : null);

        foreach (var nvp in nameValuePairs)
        {
            keys.Add(nvp.Item1);
            values.Add(nvp.Item2);
        }
    }
}
