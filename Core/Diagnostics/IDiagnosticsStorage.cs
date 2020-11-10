using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.Diagnostics
{
    public interface IDiagnosticsStorage
    {
        /// <summary> . </summary>
        string LocalStoragePath { get; }

        /// <summary> . </summary>
        Task<string> ReadAllTextAsync(string filePath);

        /// <summary> . </summary>
        bool EnsureFolderExists(string filePath);

        /// <summary> . </summary>
        Task AppendTextAsync(string filePath, string content);

        /// <summary> . </summary>
        IEnumerable<string> GetDataFiles(string folderPath);

        /// <summary> . </summary>
        bool DeleteFile(string filePath);

        /// <summary> Zips. </summary>
        byte[] ZipFiles(string[] filesToZip, string zipFilePath);
    }
}
