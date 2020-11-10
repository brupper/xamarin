using Brupper.Diagnostics;
using Brupper.IO;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Diagnostics
{
    internal class FormsStorage : AStorage, IDiagnosticsStorage
    {
        private readonly IFileSystem fileSystem;

        public override string LocalStoragePath => fileSystem.AppDataDirectory;

        public FormsStorage(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }
    }
}
