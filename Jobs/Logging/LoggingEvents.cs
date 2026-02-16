using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace Brupper.Jobs;

/// <summary> This enum represents the priority tags of a log </summary>
[Flags]
public enum LogTags
{
    None = 0,
    ServerResponse = 1 << LogTagBitPositions.ServerResponse,
    Warning = 1 << LogTagBitPositions.Warning,
    Error = 1 << LogTagBitPositions.Error,
    Critical = 1 << LogTagBitPositions.Critical,
    CameraBasic = 1 << LogTagBitPositions.CameraBasic,
    CameraExtended = 1 << LogTagBitPositions.CameraExtended,
    InternetQualityChanged = 1 << LogTagBitPositions.InternetQualityChanged,
    Location = 1 << LogTagBitPositions.Location,
    NetworkConnection = 1 << LogTagBitPositions.NetworkConnection,
    TaskChangeEvent = 1 << LogTagBitPositions.TaskChangeEvent,
    CleanUp = 1 << LogTagBitPositions.CleanUp,
    StartUp = 1 << LogTagBitPositions.StartUp,
    CanceledAction = 1 << LogTagBitPositions.CanceledAction,
    Notification = 1 << LogTagBitPositions.Notification,
    SyncStep = 1 << LogTagBitPositions.SyncStep,
    Other = 1 << LogTagBitPositions.Other,
    LocalDataChange = 1 << LogTagBitPositions.LocalDataChange,
    AppUpdate = 1 << LogTagBitPositions.AppUpdate,
    Init = 1 << LogTagBitPositions.Init,
    Recover = 1 << LogTagBitPositions.Recover,
    Storage = 1 << LogTagBitPositions.Storage,
    UserSelection = 1 << LogTagBitPositions.UserSelection,
    WebView = 1 << LogTagBitPositions.WebView,
}

public enum LogTagBitPositions
{
    ServerResponse,
    Warning,
    Error,
    Critical,
    CameraBasic,
    CameraExtended,
    InternetQualityChanged,
    Location,
    NetworkConnection,
    TaskChangeEvent,
    CleanUp,
    StartUp,
    CanceledAction,
    Notification,
    SyncStep,
    Other,
    LocalDataChange,
    AppUpdate,
    Init,
    Recover,
    Storage,
    UserSelection,
    WebView,
}

public static class LoggingEvents
{
    public static EventId AsEventId(this LogTags logTags)
    {
        return new(Convert.ToInt32(logTags), JsonSerializer.Serialize(logTags));
    }
}

