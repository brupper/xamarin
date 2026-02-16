using Brupper.Diagnostics;
using Brupper.IO;

namespace Brupper.Maui.Diagnostics;

internal class FormsStorage : AStorage, IDiagnosticsStorage
{
    private readonly IFileSystem fileSystem;

    public override string LocalStoragePath => fileSystem.AppDataDirectory;

    public FormsStorage(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }
}
