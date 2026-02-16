using Brupper.Diagnostics;

namespace Brupper.Maui.Diagnostics;

internal class FormsLogger : Logger
{
    public FormsLogger(
        IDiagnosticsStorage diagnosticsStorage
        , IDiagnosticsPlatformInformationProvider platformInformationProvider)
           : base(diagnosticsStorage, platformInformationProvider)
    {
    }
}
