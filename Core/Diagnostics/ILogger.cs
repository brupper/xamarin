using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Brupper.Diagnostics
{
    /// <summary> Provides methods to log errors and exceptions to registered <see cref="T:ILogProvider"/> instances. </summary>
    public interface ILogger
    {
        IDiagnosticsPlatformInformationProvider PlatformInformationProvider { get; }

        SystemInfo SystemInfo { get; set; }

        string CurrentUser { get; set; }

        string CurrentCustomerAccount { get; set; }

        string CurrentServer { get; set; }

        LogTag LocalLogTag { get; set; }

        LogTag RemoteLogTag { get; set; }

        string LocalLogLevelName { get; set; }

        string RemoteLogLevelName { get; set; }

        /// <summary> Logs an event. </summary>
        /// <param name="eventName">The name of the event to be logged.</param>
        /// <param name="metaData">Additional meta data associated to the event. Provided in the form of an enumeration of <see cref="T:System.Collections.Generic.KeyValuePair{string,string}"/>.</param>
        void TrackEvent(string eventName, IEnumerable<KeyValuePair<string, string>>? metaData = null);

        /// <summary> Logs a message asynchronously. </summary>
        /// <param name="tag">The incident tags.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="metaData">Additional meta data associated to the message. Provided in the form of an enumeration of <see cref="T:System.Collections.Generic.KeyValuePair{string,string}"/>.</param>
        /// <param name="memberName">The name of the calling method.</param>
        /// <param name="sourceFilePath">The of the source file of the calling method.</param>
        /// <param name="sourceLineNumber">The line number of the calling method.</param>
        /// <remarks>
        /// Classes that implement this method should make use of special attributes
        /// <see cref="T:System.Runtime.CompilerServices.CallerMemberName" />,
        /// <see cref="T:System.Runtime.CompilerServices.CallerFilePath" /> and
        /// <see cref="T:System.Runtime.CompilerServices.CallerLineNumber" /> to provide the requested values.
        /// </remarks>
        void TrackTrace(
            LogTag tag,
            string message,
            IEnumerable<KeyValuePair<string, string>>? metaData = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary> Logs an exception. </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <param name="metaData">Additional meta data associated to the exception. Provided in the form of an enumeration of <see cref="T:System.Collections.Generic.KeyValuePair{string,string}"/></param>
        /// <param name="memberName">The name of the calling method.</param>
        /// <param name="sourceFilePath">The of the source file of the calling method.</param>
        /// <param name="sourceLineNumber">The line number of the calling method.</param>
        /// <remarks>
        /// Classes that implement this method should make use of special attributes
        /// <see cref="T:System.Runtime.CompilerServices.CallerMemberName" />,
        /// <see cref="T:System.Runtime.CompilerServices.CallerFilePath" /> and
        /// <see cref="T:System.Runtime.CompilerServices.CallerLineNumber" /> to provide the requested values.
        /// </remarks>
        void TrackException(
            Exception exception,
            IEnumerable<KeyValuePair<string, string>>? metaData = null,
            LogLevel logLevel = LogLevel.All,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        /// <summary> Starts page tracking. </summary>
        /// <param name="pageName">The name of the page to be logged.</param>
        /// <param name="tag">The incident tags.</param>
        /// <param name="metaData">Additional meta data associated to the page. Provided in the form of an enumeration of <see cref="T:System.Collections.Generic.KeyValuePair{string,string}"/>.</param>
        void StartTrackPageView(string pageName, LogTag tag, IEnumerable<KeyValuePair<string, string>>? metaData = null);

        /// <summary> Stops page time tracking. </summary>
        /// <param name="pageName">The name of the page to be logged.</param>
        void StopTrackPageView(string pageName);

        /// <summary> Starts time tracking of a process. </summary>
        /// <param name="key">The key of the tracked process.</param>
        /// <param name="tag">The incident tags.</param>
        void StartTrackTime(string key, LogTag tag, string? processId = null);

        /// <summary> Stops time tracking of a process. </summary>
        /// <param name="key">The key of the tracked process.</param>
        void StopTrackTime(string key, LogTag tag, string? processId = null);

        /// <summary> Registers and creates an instance of <see cref="T:ILogProvider" /> that will listen to log submissions. </summary>
        /// <typeparam name="T">The implementation of <see cref="T:ILogProvider"/>.</typeparam>
        /// <param name="tag">The tags that will result in a message being logged to a given provider.</param>
        /// <remarks>
        /// Classes implementing 
        /// <see cref="T:ILogger" /> should maintain a collection of
        /// <see cref="T:ILogProvider" /> instances and transmit any logged errors to them depending on the
        /// <see cref="T:LogTag" /> used during registration.
        /// </remarks>
        void RegisterProvider<T>(LogTag tag) where T : ILogProvider, new();

        /// <summary> Registers an instance of <see cref="T:ILogProvider" /> that will listen to log submissions. </summary>
        /// <param name="instance">The instance of <see cref="T:ILogProvider"/> registered.</param>
        /// <param name="tag">The tags that will result in a message being logged to a given provider.</param>
        /// <remarks>
        /// Classes implementing 
        /// <see cref="T:ILogger" /> should maintain a collection of
        /// <see cref="T:ILogProvider" /> instances and transmit any logged errors to them depending on the
        /// <see cref="T:LogTag" /> used during registration.
        /// </remarks>
        void RegisterProvider(ILogProvider instance, LogTag tag);

        /// <summary> Unregisters an instance of <see cref="T:ILogProvider"/>. </summary>
        /// <typeparam name="T">The implementation of <see cref="T:ILogProvider"/>.</typeparam>
        void UnregisterProvider<T>() where T : ILogProvider, new();

        /// <summary> Gets the registered provider instance. </summary>
        /// <typeparam name="T">The type of <see cref="T:ILogProvider"/>.</typeparam>
        /// <returns>An instance of <see cref="T:ILogProvider"/>.</returns>
        T GetProvider<T>() where T : ILogProvider;

        /// <summary> </summary>
        void UpdateProvidersByConfig();

        /// <summary>
        /// Used to clean log files (platform specific) older than 'sinceNDays' days.
        /// </summary>
        /// <param name="logsFolderPath">Path to the local logs folder</param>
        /// <returns>A task that completes when the operation is finished.</returns>
        Task CleanLogFilesAsync(string logsFolderPath);
    }
}
