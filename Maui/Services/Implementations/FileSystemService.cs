using System.IO;
using System.Threading.Tasks;

namespace Brupper.Maui.Services;

[Obsolete("User MAUI storage")]
public class FileSystemService : IFileSystem
{
    public string CacheDirectory
        => FileSystem.CacheDirectory;

    public string AppDataDirectory
        => FileSystem.AppDataDirectory;

    public Task<bool> AppPackageFileExistsAsync(string filename)
        => FileSystem.AppPackageFileExistsAsync(filename);

    public Task<Stream> OpenAppPackageFileAsync(string filename)
        => FileSystem.OpenAppPackageFileAsync(filename);
}
