using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Services
{
    public class FileSystemService : IFileSystem
    {
        public string CacheDirectory
            => FileSystem.CacheDirectory;

        public string AppDataDirectory
            => FileSystem.AppDataDirectory;

        public Task<Stream> OpenAppPackageFileAsync(string filename)
            => FileSystem.OpenAppPackageFileAsync(filename);
    }
}
