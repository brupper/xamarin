using Brupper.Diagnostics;

namespace Brupper.Forms.Diagnostics
{
    internal class FormsLogger : Logger
    {
        public FormsLogger(
            IDiagnosticsStorage diagnosticsStorage
            , IDiagnosticsPlatformInformationProvider platformInformationProvider)
               : base(diagnosticsStorage, platformInformationProvider)
        {
        }
    }
}
