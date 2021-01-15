using Brupper.Forms.Models.Rendering;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Services.Concretes
{
    public abstract class AOutputRendererServices : IOutputRendererServices
    {
        protected readonly IFileSystem fileSystem;

        /// <summary> . </summary>
        public virtual string DocumentsFolder
            => fileSystem.AppDataDirectory;
        //=> Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        /// <summary> . </summary>
        public AOutputRendererServices(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary> . </summary>
        public virtual Task OpenPdfAsync(string filePath) => Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath),
        });

        /// <summary> . </summary>
        public abstract Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

        /// <summary> . </summary>
        public abstract Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);
    }
}
