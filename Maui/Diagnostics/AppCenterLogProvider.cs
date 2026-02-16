using Brupper.Diagnostics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Maui.Diagnostics;

/// <summary> A base class to provide access to AppCenter analytics provider. </summary>
public class AppCenterLogProvider : IAppCenterLogProvider, ILogProvider
{
    #region Private Field

    private const string LocalDatabaseName = "local.db";
    private const string AttachmentFileName = "data.zip";

    #endregion

    #region App Center Secret Keys

    #endregion

    #region Constructors

    public AppCenterLogProvider()
    {
        InitializeAsync().ConfigureAwait(false);
    }

    #endregion

    #region ILogProvider Implementation

    public IDiagnosticsStorage DiagnosticsStorage { get; set; }

    public async Task TrackTraceAsync(LogMessage message)
    {
        var properties = await PrepareMetaData("Trace", message);
        Analytics.TrackEvent(message.Message, properties);
    }

    public async Task TrackEventAsync(LogMessage message)
    {
        var properties = await PrepareMetaData("Event", message);
        Analytics.TrackEvent(message.EventName, properties);
    }

    public Task StartTrackPageViewAsync(string pageName, IEnumerable<KeyValuePair<string, string>> metaData = null)
    {
        TrackTimeHandler.Start(pageName);

        if (metaData != null)
        {
            Analytics.TrackEvent(pageName, metaData.ToDictionary(data => data.Key, data => data.Value));
        }

        return Task.FromResult(true);
    }

    public Task StopTrackPageViewAsync(string pageName)
    {
        var elapsedTime = TrackTimeHandler.Stop(pageName);
        var properties = new Dictionary<string, string>
        {
            { "ElapsedTime", elapsedTime.ToString() }
        };

        Analytics.TrackEvent(pageName, properties);

        return Task.FromResult(true);
    }

    public Task StartTrackTimeAsync(string key, string processId = null)
    {
        TrackTimeHandler.Start(key, processId);

        if (processId != null)
        {
            var properties = new Dictionary<string, string>
            {
                { "process_id", processId }
            };

            Analytics.TrackEvent(key, properties);
        }
        else
        {
            Analytics.TrackEvent(key);
        }

        return Task.FromResult(true);
    }

    public Task StopTrackTimeAsync(string key, string processId = null)
    {
        var elapsedTime = TrackTimeHandler.Stop(key, processId);

        var properties = new Dictionary<string, string>
        {
            { "ElapsedTime", elapsedTime.ToString() }
        };

        if (processId != null)
        {
            properties.Add("process_id", processId);
        }

        Analytics.TrackEvent(key, properties);

        return Task.FromResult(true);
    }

    public async Task TrackExceptionAsync(LogMessage message)
    {
        var properties = await PrepareMetaData("Exception", message);
        Crashes.TrackError(message.LogException, properties);
    }

    #endregion

    #region Private Methods

    private async Task InitializeAsync()
    {
        try
        {
            Crashes.GetErrorAttachments = e =>
            {
                ErrorAttachmentLog[] attachments = null;

                var data = CreateAttachmentZipFile(AttachmentFileName);
                if (data != null)
                {
                    attachments = new[]
                    {
                        ErrorAttachmentLog.AttachmentWithBinary(data, AttachmentFileName, "application/x-zip-compressed")
                    };
                }

                return attachments;
            };

            // Tetelezzuk fel hogy masol inditjak el: 
            // TODO:
            // AppCenter.Start(appCenterAppSecretAndroid + ";" + appCenterAppSecretIOS + ";" + appCenterAppSecretUWP, typeof(Analytics), typeof(Crashes));


            if (await Crashes.HasCrashedInLastSessionAsync())
            {
                var crashReport = await Crashes.GetLastSessionCrashReportAsync();
                Crashes.TrackError(crashReport.Exception);
            }
        }
        catch (Exception ex)
        {
            throw new DiagnosticsException(ex.Message, ex);
        }
    }

    private async Task<IDictionary<string, string>> PrepareMetaData(string type, LogMessage message)
    {
        var metaData = new Dictionary<string, string>
        {
            {"Type", type},
            {"Customer Account", message.CustomerAccount},
            {"User Name", message.UserName}
        };

        if (message.Tag > 0)
        {
            metaData.Add("LogTag", message.Tag.ToString());
        }

        if (message.SystemInfo != null)
        {
            //var geoPosition = message.SystemInfo.GeoPosition;
            //if (geoPosition != null)
            //{
            //    metaData.Add("GeoPosition Longitude", geoPosition.Longitude.ToString());
            //    metaData.Add("GeoPosition Latitude", geoPosition.Latitude.ToString());
            //    metaData.Add("GeoPosition Altitude", geoPosition.Altitude.ToString());
            //}

            //var networkInfo = message.SystemInfo.NetworkInfo;
            //if (networkInfo != null)
            //{
            //    string connectionTypes = string.Empty;
            //    string bandwidths = string.Empty;

            //    var succes = Task.Run(() =>
            //    {
            //        connectionTypes = string.Join("|", networkInfo.ConnectionTypes);
            //        bandwidths = string.Join("|", networkInfo.Bandwidths);
            //    }).Wait(100);

            //    if (!succes)
            //    {
            //        metaData.Add("Network Connection types", "timeout");
            //    }
            //    else
            //    {
            //        metaData.Add("Network Connection types", connectionTypes);
            //        metaData.Add("Network Bandwidths", bandwidths);
            //    }
            //}

            var serverInfo = message.SystemInfo.Server;
            if (serverInfo != null)
            {
                metaData.Add("Server", serverInfo);
            }
        }

        if (message.MetaData.Any())
        {
            foreach (var data in message.MetaData)
            {
                metaData.Add(data.Key, data.Value);
            }
        }

        if (message.LogException?.StackTrace != null)
        {
            string exceptionMessage = ProvideExceptionMessage(message.LogException);
            metaData.Add("Exception", exceptionMessage);

            if (message.LogException.InnerException != null)
            {
                string innerExceptionMessage = ProvideExceptionMessage(message.LogException.InnerException);
                metaData.Add("InnerException", innerExceptionMessage);
            }
        }

        if (!string.IsNullOrEmpty(message.SourceFilePath))
        {
            metaData.Add("SourceFilePath", message.SourceFilePath);
            metaData.Add("SourceLineNumber", message.SourceLineNumber.ToString());
        }

        var installId = await AppCenter.GetInstallIdAsync();
        metaData.Add("InstallationId", installId?.ToString() ?? "N/A");

        return metaData;
    }

    private string ProvideExceptionMessage(Exception exception)
    {
        if (exception != null)
        {
            StringBuilder messageBuilder = new StringBuilder(exception.Message);
            messageBuilder.AppendLine();

            if (exception.StackTrace != null)
            {
                var stackTrace = from line in exception.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                 select string.Format(CultureInfo.InvariantCulture, "    {0}", line);

                foreach (var line in stackTrace)
                {
                    messageBuilder.AppendLine(line);
                }
            }
            messageBuilder.Remove(messageBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return messageBuilder.ToString();
        }

        return "The exception was null";
    }

    private byte[] CreateAttachmentZipFile(string zipFileName)
    {
        var storagePath = DiagnosticsStorage.LocalStoragePath;
        var zipFilePath = Path.Combine(storagePath, zipFileName);

        //var databasePath = Path.Combine(storagePath, LocalDatabaseName);
        var logFilesPath = Path.Combine(storagePath, LocalStorageLogProvider.LocalLogFolder);
        var logFileToZip = Directory.GetFiles(logFilesPath).OrderByDescending(File.GetLastWriteTime).FirstOrDefault();

        return DiagnosticsStorage.ZipFiles(new[] { /*databasePath,*/ logFileToZip }, zipFilePath);
    }

    #endregion
}
