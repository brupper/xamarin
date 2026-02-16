using Brupper.Maui.Models.Rendering;
using Brupper.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace Brupper.Maui.Services.Concretes;

public abstract class AOutputRendererServices : IOutputRendererServices
{
    protected readonly IFileSystem fileSystem;

    /// <inheritdoc />
    public virtual string DocumentsFolder
        => fileSystem.AppDataDirectory;
    //=> Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <inheritdoc />
    public AOutputRendererServices(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    /// <inheritdoc />
    public virtual Task OpenPdfAsync(string filePath) => Launcher.OpenAsync(new OpenFileRequest
    {
        File = new ReadOnlyFile(filePath),
    });

    /// <inheritdoc />
    public abstract Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

    /// <inheritdoc />
    public abstract Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);
}
