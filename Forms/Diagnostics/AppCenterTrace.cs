using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Brupper
{
    public static class LoggerExtensions
    {
        public static void TrackError(this ILogger logger, Exception exception, IDictionary<string, string> properties = null, params ErrorAttachmentLog[] attachments)
        {
            Debug.WriteLine(exception);
            Crashes.TrackError(exception, properties, attachments);
        }

        public static void TrackEvent(this ILogger logger, string name, IDictionary<string, string> properties = null)
        {
            Analytics.TrackEvent(name, properties);
        }
    }

    class AppCenterLoggerFactory : ILoggerFactory
    {
        private readonly List<ILoggerProvider> _providerCollection = new List<ILoggerProvider>();
        private readonly ILoggerProvider provider;

        public AppCenterLoggerFactory(ILoggerProvider provider) => this.provider = provider;

        /// <summary> Disposes the provider. </summary>
        public void Dispose() => provider.Dispose();

        /// <summary> Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance. </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        public ILogger CreateLogger(string categoryName)
        {
            return provider.CreateLogger(categoryName);
        }

        /// <summary> Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system. </summary>
        public void AddProvider(ILoggerProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (_providerCollection != null)
                _providerCollection.Add(provider);
            else
                Debug.WriteLine("Ignoring added logger provider {0}", provider);
        }
    }

    class AppCenterLoggerProvider : ILoggerProvider
    {
        #region Fields
        #endregion

        #region Constructor

        public AppCenterLoggerProvider() { }

        #endregion

        #region ILoggerProvider

        public ILogger CreateLogger(string name) => new AppCenterTrace(name);

        #endregion

        #region IDisposable

        ~AppCenterLoggerProvider()
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

    public class AppCenterTrace : ILogger
    {
        private readonly string name;

        public AppCenterTrace(string name) => this.name = name;

        #region ILogger

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = "{0}";
            try
            {
                message = $"[{eventId.Id,2}: {logLevel,-12}]\t{name} - {formatter(state, exception)}";

                Debug.WriteLine(message);

                if (exception != null)
                {
                    Crashes.TrackError(exception);
                }
                else if (logLevel > LogLevel.Information)
                {
                    Analytics.TrackEvent(message);
                }
            }
            catch (FormatException formatException)
            {
                Debug.WriteLine($"Exception during trace of {message} {logLevel} {formatException}");
                Crashes.TrackError(formatException);
            }
            catch (Exception unhandled)
            {
                Crashes.TrackError(unhandled);
            }
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => true; //WrappedLog?.IsEnabled(logLevel) ?? false;

        #endregion
    }
}
