using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class TempDataExtensions
{
    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        => tempData[key] = JsonConvert.SerializeObject(value);

    public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
        => tempData.TryGetValue(key, out var o) ? JsonConvert.DeserializeObject<T>(o as string ?? string.Empty) : null;

    public static string? GetAsString(this ITempDataDictionary tempData, string key)
        => tempData.TryGetValue(key, out var o) ? (string?)o : null;
}
