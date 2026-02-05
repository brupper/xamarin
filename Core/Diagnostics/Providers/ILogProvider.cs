using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    /// <summary> Interface for all the provider of each type which are registered by the main logger. </summary>
    public interface ILogProvider
    {
        /// <summary> Sends a trace message asynchronously. </summary>
        Task TrackTraceAsync(LogMessage message);

        /// <summary> Sends an event message asynchronously. </summary>
        Task TrackEventAsync(LogMessage message);

        /// <summary> Starts tracking spent time on page view asynchronously. </summary>
        /// <param name="pageName">The name of the page to be logged.</param>
        /// <param name="metaData">Additional meta data associated to the page. Provided in the form of an enumeration of
        /// <see cref="T:System.Collections.Generic.KeyValuePair{string,string}"/>.</param>
        Task StartTrackPageViewAsync(string pageName, IEnumerable<KeyValuePair<string, string>>? metaData = null);

        /// <summary> Stops page time tracking asynchronously. </summary>
        /// <param name="pageName">The name of the page to be logged.</param>
        Task StopTrackPageViewAsync(string pageName);

        /// <summary> Starts time tracking of a process asynchronously. </summary>
        Task StartTrackTimeAsync(string key, string? processId = null);

        /// <summary> Stops time tracking of a process asynchronously. </summary>
        Task StopTrackTimeAsync(string key, string? processId = null);

        /// <summary> Sends an exception message asynchronously. </summary>
        /// <param name="message">The message to send.</param>
        Task TrackExceptionAsync(LogMessage message);
    }
}
