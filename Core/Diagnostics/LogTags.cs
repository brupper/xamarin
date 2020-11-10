using System;

namespace Brupper.Diagnostics
{
    /// <summary> This enum represents the priority tags of a log </summary>
    [Flags]
    public enum LogTag
    {
        None = 0,
        ServerResponse = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
        Critical = 1 << 3,
        InternetQualityChanged = 1 << 4,
        NetworkConnection = 1 << 5,
        CleanUp = 1 << 6,
        StartUp = 1 << 7,
        CanceledAction = 1 << 8,
        Notification = 1 << 9,
        SyncStep = 1 << 10,
        Other = 1 << 11,
        LocalDataChange = 1 << 12,
        AppUpdate = 1 << 13,
        Init = 1 << 14,
        Recover = 1 << 15,
        Storage = 1 << 16,
        UserSelection = 1 << 17,
    }
}
