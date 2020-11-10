using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.IO
{
    public interface IStorage
    {
        /// <summary> Gets the local storage path. </summary>
        string LocalStoragePath { get; }

        /// <summary> Checks file exists or not in LocalStorage folder or subfolder, depends on the second parameter. </summary>
        bool FileExists(string fileName);

        /// <summary> Indicates if a folder exists (async) </summary>
        bool FolderExists(string folderName);

        /// <summary> Ensure a file in local path exists </summary>
        bool EnsureFileExists(string fileName);

        /// <summary> Ensure a folder in local path exists </summary>
        bool EnsureFolderExists(string folderName);

        /// <summary> Creates a folder </summary>
        string CreateFolder(string folderName);

        /// <summary> Creates a file </summary>
        string CreateFile(string filename);

        /// <summary> Writes all text to a target file. </summary>
        Task<bool> WriteTextAllAsync(string filename, string content);

        /// <summary> Writes text to the end of the target file. </summary>
        Task AppendTextAsync(string filename, string content = "");

        /// <summary> Reads a text from a file </summary>
        Task<string> ReadAllTextAsync(string fileName);

        /// <summary> Deletes a file. </summary>
        bool DeleteFile(string fileName);

        /// <summary> Deletes a folder. </summary>
        bool DeleteFolder(string folderName);

        /// <summary> Saves a byte array </summary>
        Task<bool> SaveByteArray(byte[] array, string fileName);

        /// <summary> Loads a byte array. </summary>
        Task<byte[]> LoadByteArray(string fileName);

        /// <summary> Gets a list of file names contained in the desired path. </summary>
        IEnumerable<string> GetDataFiles(string folderName);

        /// <summary> Gets a list of folder names contained in the desired path. </summary>
        IEnumerable<string> GetDataFolders(string folderName);

        /// <summary> Zips. </summary>
        byte[] ZipFiles(string[] filesToZip, string zipFilePath);
    }
}
