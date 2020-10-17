using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Services.Concretes
{
    public abstract class AOutputRendererServices : IOutputRendererServices
    {
        protected readonly IFileSystem fileSystem;

        public string DocumentsFolder
            => fileSystem.AppDataDirectory;
        //=> Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public AOutputRendererServices(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public abstract Task OpenPdfAsync(string filePath);

        public abstract Task<string> SaveIntoPdfAsync(string htmlContent, string fileName);

        public abstract Task<string> SaveIntoPngAsync(string htmlContent, string fileName);
    }
}
