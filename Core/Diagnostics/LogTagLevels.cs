
namespace Brupper.Diagnostics
{
    public class LogTagLevels
    {
        public const LogTag Low = LogTag.Warning | LogTag.Error | LogTag.Notification | LogTag.SyncStep | LogTag.Init | LogTag.Recover | LogTag.Storage | LogTag.UserSelection | LogTag.StartUp;
        public const LogTag Medium = Low | LogTag.Critical | LogTag.CleanUp | LogTag.StartUp | LogTag.Other | LogTag.LocalDataChange | LogTag.AppUpdate;
        public const LogTag High = Medium | LogTag.ServerResponse | LogTag.NetworkConnection;
        public const LogTag All = High | LogTag.InternetQualityChanged | LogTag.CanceledAction;
    }
}
