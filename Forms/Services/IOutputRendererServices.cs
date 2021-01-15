using Brupper.Forms.Models.Rendering;
using System.Threading.Tasks;

namespace Brupper.Forms.Services
{
    public interface IOutputRendererServices
    {
        string DocumentsFolder { get; }

        Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

        Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

        Task OpenPdfAsync(string filePath);
    }
}
