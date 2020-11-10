using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Brupper.Diagnostics
{
    /*
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IDiagnosticsPlatformInformationProvider, PlatformInformationProvider>();
        Mvx.IoCProvider.ConstructAndRegisterSingleton<IDiagnosticsStorage, FormsStorage>();
        Logger.Current.RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
        Logger.Init<FormsLogger>(Mvx.IoCProvider.IoCConstruct<FormsLogger>());
     */

    /// <summary>
    /// <para>A base logger class, responsible for sending <see cref="T:LogMessage" /> instances to the proper registered
    /// <see cref="T:ILogProvider" /> instances.</para>
    /// <para>Since <see cref="T:LogMessage" /> instances are build with platform-specific code, each platform
    /// should extend this class and implement its abstract methods.</para>
    /// <inheritdoc />
    /// </summary>
    public abstract class Logger : ILogger
    {
        public static ILogger Current { get; internal set; }

        private readonly Dictionary<ILogProvider, LogTag> logProviders
            = new Dictionary<ILogProvider, LogTag>();

        private readonly IDictionary<string, Guid> ProcessIds = new Dictionary<string, Guid>();

        private readonly IDictionary<Guid, bool> RunningProcess = new Dictionary<Guid, bool>();

        private readonly IDiagnosticsStorage diagnosticsStorage;

        public IDiagnosticsPlatformInformationProvider PlatformInformationProvider { get; }

        public LoggerSettings Settings { get; }
            = new LoggerSettings();

        #region Constructor

        protected Logger(IDiagnosticsStorage diagnosticsStorage, IDiagnosticsPlatformInformationProvider platformInformationProvider)
        {
            this.diagnosticsStorage = diagnosticsStorage;
            this.PlatformInformationProvider = platformInformationProvider;
        }

        #endregion

        /// <summary> var logger = Mvx.IoCProvider.IoCConstruct T(); </summary>
        public static void Init<T>(T logger) where T : Logger
        {
            if (Logger.Current == null)
            {
                logger.RegisterProviders();
                Logger.Current = logger;

                Debug.WriteLine($"Storage full path is: {logger.diagnosticsStorage?.LocalStoragePath}");
            }
        }

        protected virtual void RegisterProviders()
        {
            SystemInfo = new SystemInfo(GetOperatingSystem(),
                                        GetMachineName(),
                                        GetAppVersion(),
                                        CurrentServer);

            //RegisterProvider<AppCenterLogProvider>(LogTagLevels.Medium);
            RegisterProvider<LocalStorageLogProvider>(LogTagLevels.Medium);

#if DEBUG
            RegisterProvider<ConsoleLogProvider>(LogTagLevels.All);
#endif
        }

        public SystemInfo SystemInfo { get; set; }

        public string CurrentUser { get; set; }

        public string CurrentCustomerAccount { get; set; }

        public string CurrentServer { get; set; }

        public LogTag LocalLogTag { get; set; }

        public LogTag RemoteLogTag { get; set; }

        public string LocalLogLevelName { get; set; }

        public string RemoteLogLevelName { get; set; }

        public void TrackTrace(
            LogTag tag,
            string message,
            IEnumerable<KeyValuePair<string, string>> metaData = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = -1)
        {
            var logMessage = BuildLogMessage(tag, message, memberName, sourceFilePath, sourceLineNumber, metaData);

            for (int i = 0; i < logProviders.Count; i++)
            {
                var logProvider = logProviders.ElementAt(i);
                if (logProvider.Value.HasFlag(tag))
                {
                    _ = logProvider.Key.TrackTraceAsync(logMessage).ConfigureAwait(false);
                }
            }
        }

        public void TrackException(
            Exception exception,
            IEnumerable<KeyValuePair<string, string>> metaData = null,
            LogLevel logLevel = LogLevel.All,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (exception != null)
            {
                for (int i = 0; i < logProviders.Count; i++)
                {
                    var logProvider = logProviders.ElementAt(i);

                    var logMessage = BuildLogMessage(exception, logLevel, logProvider, memberName, sourceFilePath, sourceLineNumber, metaData ?? Enumerable.Empty<KeyValuePair<string, string>>());

                    if (logProvider.Value.HasFlag(logMessage.Tag))
                    {
                        _ = logProvider.Key.TrackExceptionAsync(logMessage).ConfigureAwait(false);
                    }
                }
            }
        }

        public void RegisterProvider<T>(LogTag tag) where T : ILogProvider, new()
        {
            RegisterProvider(new T(), tag);
        }

        public void RegisterProvider(ILogProvider instance, LogTag tag)
        {
            if (instance is ILocalStorageLogProvider storageLogger)
            {
                storageLogger.DiagnosticsStorage = diagnosticsStorage;
                storageLogger.DiagnosticsStorage.EnsureFolderExists(storageLogger.Folder);
            }
            else if (instance is IAppCenterLogProvider appCenterLogger)
            {
                appCenterLogger.DiagnosticsStorage = diagnosticsStorage;
            }

            logProviders.Add(instance, tag);
        }

        public void UnregisterProvider<T>() where T : ILogProvider, new()
        {
            var logProvider = GetProvider<T>();
            if (logProvider != null)
            {
                logProviders.Remove(logProvider);
            }
        }

        public T GetProvider<T>() where T : ILogProvider
        {
            var logProvider = logProviders.Keys.FirstOrDefault(p => p is T);

            return logProvider != null ? (T)logProvider : default(T);
        }

        public Task CleanLogFilesAsync(string logsFolderPath)
        {
            return Task.Run(() =>
            {
                var logFileNames = diagnosticsStorage.GetDataFiles(logsFolderPath);
                foreach (var logFileName in logFileNames)
                {
                    diagnosticsStorage.DeleteFile(logFileName);
                }
            });
        }

        protected virtual string GetAppVersion()
            => PlatformInformationProvider.GetVersion();

        protected virtual string GetMachineName()
            => PlatformInformationProvider.GetMachineName();

        protected virtual string GetOperatingSystem()
            => string.Format(CultureInfo.InvariantCulture, $"{PlatformInformationProvider.GetPlatform()} {PlatformInformationProvider.GetOSVersion()}");

        protected Task SendFatalExceptionAsync(
            Exception e,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            TrackException(e, null, LogLevel.All, memberName, sourceFilePath, sourceLineNumber);

            return Task.FromResult<object>(null);
        }

        //// Build a message for the logger based on a custom message from developer
        private LogMessage BuildLogMessage(
            LogTag tag,
            string message,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber,
            IEnumerable<KeyValuePair<string, string>> metaData)
        {
            var msg = BuildBaseLogMessage(memberName, sourceFilePath, sourceLineNumber, metaData);

            msg.Tag = tag;
            msg.Message = message;

            return msg;
        }

        //// Build a message for the logger based on an exception caught by the app
        private LogMessage BuildLogMessage(
            Exception exception,
            LogLevel logLevel,
            KeyValuePair<ILogProvider, LogTag> logProvider,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber,
            IEnumerable<KeyValuePair<string, string>> metaData)
        {
            var msg = BuildBaseLogMessage(memberName, sourceFilePath, sourceLineNumber, metaData);

            msg.Tag = LogTag.Error;
            msg.Message = exception.Message;

            if ((logProvider.Key is IAppCenterLogProvider) || logLevel != LogLevel.Low)
            {
                msg.LogException = exception;
            }

            return msg;
        }

        //// Build the common base to the different type of log message
        private LogMessage BuildBaseLogMessage(
            string memberName,
            string sourceFilePath,
            int sourceLineNumber,
            IEnumerable<KeyValuePair<string, string>> metaData)
        {
            var msg = new LogMessage(SystemInfo)
            {
                CustomerAccount = CurrentCustomerAccount,
                UserName = CurrentUser,
                MemberName = memberName,
                SourceFilePath = sourceFilePath,
                SourceLineNumber = sourceLineNumber
            };

            if ((metaData ?? Enumerable.Empty<KeyValuePair<string, string>>()).Any())
            {
                msg.MetaData = metaData;
            }

            return msg;
        }

        public void TrackEvent(string eventName, IEnumerable<KeyValuePair<string, string>> metaData = null)
        {
            var msg = BuildBaseLogMessage(null, null, 0, metaData);
            msg.EventName = eventName;

            for (int i = 0; i < logProviders.Count; i++)
            {
                var logProvider = logProviders.ElementAt(i);
                _ = logProvider.Key.TrackEventAsync(msg).ConfigureAwait(false);
            }
        }

        public void StartTrackPageView(string pageName, LogTag tag, IEnumerable<KeyValuePair<string, string>> metaData = null)
        {
            for (int i = 0; i < logProviders.Count; i++)
            {
                var logProvider = logProviders.ElementAt(i);
                if (logProvider.Value.HasFlag(tag))
                {
                    _ = logProvider.Key.StartTrackPageViewAsync(pageName, metaData ?? Enumerable.Empty<KeyValuePair<string, string>>()).ConfigureAwait(false);
                }
            }
        }

        public void StopTrackPageView(string pageName)
        {
            for (int i = 0; i < logProviders.Count; i++)
            {
                var logProvider = logProviders.ElementAt(i);
                _ = logProvider.Key.StopTrackPageViewAsync(pageName).ConfigureAwait(false);
            }
        }

        public void StartTrackTime(string key, LogTag tag, string processId = null)
        {
            Guid processGuid = Guid.NewGuid();

            string recordkey = processId ?? key;

            for (int i = 0; i < logProviders.Count; i++)
            {
                var logProvider = logProviders.ElementAt(i);
                if (logProvider.Value.HasFlag(tag))
                {
                    ProcessIds[recordkey] = processGuid;
                    RunningProcess[processGuid] = true;

                    _ = logProvider.Key.StartTrackTimeAsync(key, processId).ConfigureAwait(false);
                }
            }
        }

        public void StopTrackTime(string key, LogTag tag, string processId = null)
        {
            string recordkey = processId ?? key;

            if (ProcessIds.ContainsKey(recordkey))
            {
                bool hasRunningProcess = RunningProcess[ProcessIds[recordkey]];

                if (hasRunningProcess)
                {
                    for (int i = 0; i < logProviders.Count; i++)
                    {
                        var logProvider = logProviders.ElementAt(i);

                        if (logProvider.Value.HasFlag(tag))
                        {
                            _ = logProvider.Key.StopTrackTimeAsync(key, processId).ConfigureAwait(false);
                        }
                    }

                    RunningProcess[ProcessIds[recordkey]] = false;
                }
            }
        }

        public void UpdateProvidersByConfig()
        {
            var localStorageLogProvider = GetProvider<LocalStorageLogProvider>();
            if (localStorageLogProvider != null)
            {
                var localStorageLogProviderLogTag = logProviders[localStorageLogProvider];

                if (localStorageLogProviderLogTag != LocalLogTag)
                {
                    logProviders[localStorageLogProvider] = LocalLogTag;
                }
            }

            var appCenterLogProvider = GetProvider<IAppCenterLogProvider>();
            if (appCenterLogProvider != null)
            {
                var appCenterLogProviderLogTag = logProviders[appCenterLogProvider];
                if (appCenterLogProviderLogTag != RemoteLogTag)
                {
                    logProviders[appCenterLogProvider] = RemoteLogTag;
                }
            }
        }
    }
}
