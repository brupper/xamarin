using System.Threading.Tasks;

namespace Brupper.Forms.Services
{
    public interface IOutputRendererServices
    {
        string DocumentsFolder { get; }

        Task<string> SaveIntoPdfAsync(string htmlContent, string fileName);

        Task<string> SaveIntoPngAsync(string htmlContent, string fileName);

        Task OpenPdfAsync(string filePath);
    }
}
