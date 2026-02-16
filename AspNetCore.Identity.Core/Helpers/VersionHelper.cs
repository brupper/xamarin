using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

internal static class VersionHelper
{
    private static string? cachedCurrentVersion;

    /// <summary> Return the Current Version from the AssemblyInfo.cs file. </summary>
    public static string CurrentVersion(this IHtmlHelper<dynamic> helper)
    {
        try
        {
            if (cachedCurrentVersion != null)
            {
                return cachedCurrentVersion;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
            return cachedCurrentVersion = $"v{version.Major}.{version.Minor}.{version.Build} - {ThisAssembly.Git.Commit}";
        }
        catch
        {
            return "?.?.?.?";
        }
    }
}
