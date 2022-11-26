using System.IO;
using System.Reflection;

public static class AssemblyExtensions
{
    public static string GetEmbeddedResourceFromResourcesAsString(this string fileName, Assembly assembly, string defaultPath = null /*"Brupper.Core.Resources"*/)
    {
        //var assembly = typeof(ReflectionUtil).GetTypeInfo().Assembly;
        using (var stream = assembly.GetManifestResourceStream($"{defaultPath}.{fileName}"))
        using (var reader = new StreamReader(stream))
        {
            var stringContent = reader.ReadToEnd();
            return stringContent;
        }
    }

    public static Stream GetEmbeddedResourceFromResourcesAsStream(this string fileName, Assembly assembly, string defaultPath = null /*"Brupper.Core.Resources"*/)
    {
        //var assembly = typeof(ReflectionUtil).GetTypeInfo().Assembly;
        using (var stream = assembly.GetManifestResourceStream($"{defaultPath}.{fileName}"))
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
