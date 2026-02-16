using Brupper.Maui.Models.Rendering;
using System.Threading.Tasks;

namespace Brupper.Maui.Services;

/// <summary> . </summary>
public interface IOutputRendererServices
{
    /// <summary> . </summary>
    string DocumentsFolder { get; }

    /// <summary>
    /// .
    /// </summary>
    /// <param name="htmlContent"></param>
    /// <param name="fileName">RELATIVE</param>
    /// <param name="kind"></param>
    /// <param name="numberOfPages"></param>
    /// <returns></returns>
    Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

    /// <summary>
    /// .
    /// </summary>
    /// <param name="htmlContent"></param>
    /// <param name="fileName">RELATIVE</param>
    /// <param name="kind"></param>
    /// <param name="numberOfPages"></param>
    /// <returns></returns>
    Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1);

    /// <summary> . </summary>
    Task OpenPdfAsync(string filePath);
}
