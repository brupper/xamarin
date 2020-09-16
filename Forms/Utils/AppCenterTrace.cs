using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Brupper
{
    public static class MvxLogExtensions
    {
        public static void TrackError(this IMvxLog logger, Exception exception, IDictionary<string, string> properties = null, params ErrorAttachmentLog[] attachments)
        {
            Crashes.TrackError(exception, properties, attachments);
        }

        public static void TrackEvent(this IMvxLog logger, string name, IDictionary<string, string> properties = null)
        {
            Analytics.TrackEvent(name, properties);
        }
    }

    public class AppCenterTrace : IMvxLog, IMvxLogProvider, IDisposable
    {
        #region Fields

        private readonly IMvxLogProvider wrapperdLogProvider;

        #endregion

        #region Constructor

        public AppCenterTrace(IMvxLogProvider wrapperdLogProvider)
        {
            this.wrapperdLogProvider = wrapperdLogProvider;
        }

        #endregion

        #region Properties

        private IMvxLog WrappedLog => wrapperdLogProvider?.GetLogFor("AppCenter");

        #endregion

        #region IMvxLog

        public bool Log(MvxLogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            var message = "{0}";
            try
            {
                message = messageFunc?.Invoke();
                WrappedLog?.Log(logLevel, messageFunc, exception, formatParameters);

                var formattedMessage = string.Format($"[{logLevel}] : {message}", formatParameters);
                Debug.WriteLine(formattedMessage);

                if (exception != null)
                {
                    Crashes.TrackError(exception);
                }
                else if (logLevel > MvxLogLevel.Info)
                {
                    Analytics.TrackEvent(formattedMessage);
                }
            }
            catch (FormatException formatException)
            {
                Log(MvxLogLevel.Error, () => $"Exception during trace of {message} {logLevel}");
                Crashes.TrackError(formatException);
            }
            catch (Exception unhandled)
            {
                Crashes.TrackError(unhandled);
            }

            return true;
        }

        public bool IsLogLevelEnabled(MvxLogLevel logLevel)
        {
            return WrappedLog?.IsLogLevelEnabled(logLevel) ?? false;
        }

        #endregion

        #region IMvxLogProvider

        public IMvxLog GetLogFor(Type type) => this;

        public IMvxLog GetLogFor<T>() => this;

        public IMvxLog GetLogFor(string name) => this;

        public IDisposable OpenNestedContext(string message) => this;

        public IDisposable OpenMappedContext(string key, string value) => this;

        #endregion

        #region IDisposable

        ~AppCenterTrace()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
