using Brupper.Diagnostics;

namespace Brupper.Forms.Diagnostics
{
    public class DefaultLogger : Logger
    {
        public DefaultLogger(
            IDiagnosticsStorage diagnosticsStorage
            , IDiagnosticsPlatformInformationProvider platformInformationProvider)
               : base(diagnosticsStorage, platformInformationProvider)
        {
        }
    }
}
