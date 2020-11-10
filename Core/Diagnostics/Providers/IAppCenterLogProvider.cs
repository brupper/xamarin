namespace Brupper.Diagnostics
{
    public interface IAppCenterLogProvider : ILogProvider
    {
        IDiagnosticsStorage DiagnosticsStorage { get; set; }
    }
}
