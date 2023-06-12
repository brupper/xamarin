using Brupper.Diagnostics;
using Brupper.IO;
using System.IO;

namespace Brupper.Forms.Diagnostics
{
    public class DefaultStorage : AStorage, IDiagnosticsStorage
    {
        public override string LocalStoragePath => LogDirectory;

        public string LogDirectory { get; set; }
         = Directory.GetCurrentDirectory();

        public DefaultStorage() { }
    }
}
