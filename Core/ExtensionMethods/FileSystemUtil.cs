using System.IO;

namespace Brupper;

public static class FileSystemExtensions
{
    public static long GetDirectorySize(this string path)
    {
        // 1. Get array of all file names.
        var a = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        // 2. Calculate total bytes of all files in a loop.
        var b = 0L;
        foreach (var name in a)
        {
            // 3.
            // Use FileInfo to get length of each file.
            var info = new FileInfo(name);
            b += info.Length;
        }

        // 4.
        // Return total size
        return b;
    }
}
