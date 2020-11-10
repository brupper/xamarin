using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    /// <summary> Provides methods to manage log file in local storage. </summary>
    public interface ILocalStorageLogProvider : ILogProvider
    {
        /// <summary> Gets the name of the log folder. </summary>
        string Folder { get; }

        /// <summary> Reads the contents of the log file. </summary>
        /// <param name="deleteAfterRead">Indicates whether log file is deleting or not after read.</param>
        /// <returns>The contents of the log file as a string.</returns>
        Task<string> ReadLogFile(bool deleteAfterRead = false);

        /// <summary> . </summary>
        public IDiagnosticsStorage DiagnosticsStorage { get; set; }
    }
}
