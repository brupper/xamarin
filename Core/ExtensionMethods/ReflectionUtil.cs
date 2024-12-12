using System;
using System.IO;
using System.Reflection;

namespace Brupper;

/// <summary> /// </summary>
public static class ReflectionUtil
{
    /// <summary> . </summary>
    public static string GetEmbeddedResourceFromResourcesAsString(this string fileName, Type typeInAssembly)
    {
        var assembly = typeInAssembly.GetTypeInfo().Assembly;
        using (var stream = assembly.GetManifestResourceStream(fileName)) // $"... .Data.Resources.{fileName}"
        using (var reader = new StreamReader(stream))
        {
            var stringContent = reader.ReadToEnd();
            return stringContent;
        }
    }

    /// <summary> . </summary>
    public static Stream GetEmbeddedResourceFromResourcesAsStream(this string fileName, Type typeInAssembly)
    {
        var assembly = typeInAssembly.GetTypeInfo().Assembly;
        using (var stream = assembly.GetManifestResourceStream(fileName)) // $"... .Data.Resources.{fileName}"
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
