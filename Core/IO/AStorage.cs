using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.IO
{
    internal class FileLocker
    {
        internal static readonly SemaphoreSlim fileWriteLock = new SemaphoreSlim(initialCount: 1);
    }

    public abstract class AStorage : IStorage
    {
        protected readonly SemaphoreSlim fileWriteLock;

        protected AStorage()
        {
            fileWriteLock = FileLocker.fileWriteLock;
        }

        public bool FileExists(string fileName)
        {
            var folder = LocalStoragePath;

            var exists = File.Exists(Path.Combine(folder, fileName));
            return exists;
        }

        public bool FolderExists(string folderName)
        {
            var folder = LocalStoragePath;

            var exists = Directory.Exists(Path.Combine(folder, folderName));
            return exists;
        }

        public bool EnsureFileExists(string fileName)
        {
            try
            {
                var isExist = FileExists(fileName);
                if (isExist)
                {
                    return true;
                }

                InternalCreateFile(fileName);
                return true;
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public bool EnsureFolderExists(string folderName)
        {
            try
            {
                fileWriteLock.Wait();
                return InternalEnsureFolderExists(folderName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public string CreateFolder(string folderName)
        {
            try
            {
                fileWriteLock.Wait();
                return InternalCreateFolder(folderName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public string CreateFile(string fileName)
        {
            try
            {
                fileWriteLock.Wait();
                return InternalCreateFile(fileName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public async Task<bool> WriteTextAllAsync(string fileName, string content)
        {
            try
            {
                await fileWriteLock.WaitAsync();
                return InternalWriteTextAll(fileName, content);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public async Task<string> ReadAllTextAsync(string fileName)
        {
            try
            {
                await fileWriteLock.WaitAsync();
                return InternalReadAllText(fileName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                fileWriteLock.Wait();
                return InternalDeleteFile(fileName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public bool DeleteFolder(string folderName)
        {
            try
            {
                fileWriteLock.Wait();
                return InternalDeleteFolder(folderName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public async Task<bool> SaveByteArray(byte[] array, string fileName)
        {
            try
            {
                await fileWriteLock.WaitAsync();
                return await InternalSaveByteArray(array, fileName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public async Task<byte[]> LoadByteArray(string fileName)
        {
            try
            {
                await fileWriteLock.WaitAsync();
                return await InternalLoadByteArray(fileName);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public IEnumerable<string> GetDataFiles(string folderName)
        {
            var localfolder = LocalStoragePath;
            var exist = FolderExists(folderName);
            if (exist)
            {
                var folder = Path.Combine(localfolder, folderName);
                foreach (var item in Directory.EnumerateFiles(folder))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<string> GetDataFolders(string folderName)
        {
            var localfolder = LocalStoragePath;
            var exist = FolderExists(folderName);
            if (exist)
            {
                var folder = Path.Combine(localfolder, folderName);
                foreach (var item in Directory.EnumerateDirectories(folder))
                {
                    yield return item;
                }
            }
        }

        public async Task AppendTextAsync(string filename, string content = "")
        {
            try
            {
                await fileWriteLock.WaitAsync();
                var folder = LocalStoragePath;
                var filePath = Path.Combine(folder, filename);
                File.AppendAllText(filePath, content);
            }
            finally
            {
                fileWriteLock.Release();
            }
        }

        public abstract string LocalStoragePath { get; }

        #region Internal methods

        protected bool InternalEnsureFolderExists(string folderName)
        {
            var isExist = FolderExists(folderName);
            if (isExist)
            {
                return true;
            }

            InternalCreateFolder(folderName);
            return true;
        }

        protected string InternalCreateFolder(string folderName)
        {
            var folder = Path.Combine(LocalStoragePath, folderName);
            Directory.CreateDirectory(folder);

            return folder;
        }

        protected string InternalCreateFile(string fileName)
        {
            var targetFolder = LocalStoragePath;
            var realFileName = Path.GetFileName(fileName);
            var relativeFolder = fileName?.Replace(realFileName ?? string.Empty, "");

            if (!string.IsNullOrEmpty(relativeFolder))
            {
                targetFolder = !string.IsNullOrEmpty(targetFolder)
                    ? Path.Combine(targetFolder, relativeFolder)
                    : relativeFolder;
            }

            if (!string.IsNullOrEmpty(targetFolder))
            {
                InternalEnsureFolderExists(targetFolder);
            }

            var path = Path.Combine(targetFolder, fileName);
            using (File.Create(path)) { }

            return path;
        }

        protected bool InternalWriteTextAll(string fileName, string content = "")
        {
            var file = InternalCreateFile(fileName);

            File.WriteAllText(file, content);

            return true;
        }

        protected string InternalReadAllText(string fileName)
        {
            string content = "";
            var folder = LocalStoragePath;
            var exist = FileExists(fileName);
            if (exist)
            {
                var path = Path.Combine(folder, fileName);
                using (var stream = File.Open(path, FileMode.Open))
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }

        protected bool InternalDeleteFile(string fileName)
        {
            var folder = LocalStoragePath;
            var exist = FileExists(fileName);
            if (exist)
            {
                var path = Path.Combine(folder, fileName);
                File.Delete(path);

                return true;
            }
            return false;
        }

        protected bool InternalDeleteFolder(string folderName)
        {
            var folder = LocalStoragePath;
            var exist = FolderExists(folderName);
            if (exist)
            {
                var path = Path.Combine(folder, folderName);
                Directory.Delete(path, true);
                return true;
            }
            return false;
        }

        protected async Task<bool> InternalSaveByteArray(byte[] array, string fileName)
        {
            var folder = LocalStoragePath;
            var filePath = Path.Combine(folder, fileName);

            EnsureFileExists(fileName);
            using (var stream = File.Open(filePath, FileMode.Truncate))
            {
                await stream.WriteAsync(array, 0, array.Length);
            }

            return true;
        }

        protected async Task<byte[]> InternalLoadByteArray(string fileName)
        {
            var folder = LocalStoragePath;
            var filePath = Path.Combine(folder, fileName);

            EnsureFileExists(fileName);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        #endregion
    }
}
