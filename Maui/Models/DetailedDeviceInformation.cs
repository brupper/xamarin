namespace Brupper.Maui.Models;

/// <summary> https://forums.xamarin.com/discussion/42727/working-with-disk-space-info-in-forms </summary>
public class DetailedDeviceInformation
{
    /// <summary>
    /// Current battery level 0 - 100
    /// </summary>
    public int BatteryRemainingChargePercent { get; internal set; }

    /// <summary>
    /// Current battery status like Charging, Discharging, etc.
    /// </summary>
    public string BatteryStatus { get; internal set; }

    /// <summary>
    /// Available RAM memory (in bytes).
    /// </summary>
    public long AvailableMainMemory { get; internal set; }

    /// <summary>
    /// Total RAM memory (in bytes).
    /// </summary>
    public long TotalMainMemory { get; internal set; }

    /// <summary>
    /// If <c>true</c> indicates that the system is low in memory.
    /// </summary>
    public bool IsLowMainMemory { get; internal set; }

    /// <summary>
    /// Total size (in bytes) of the internal storage.
    /// </summary>
    public long TotalInternalStorage { get; internal set; }

    /// <summary>
    /// Free size (in bytes) in the internal storage.
    /// It might be different than available size.
    /// </summary>
    public long FreeInternalStorage { get; internal set; }

    /// <summary>
    /// Available size (in bytes) in the internal storage.
    /// It might be different than free size.
    /// </summary>
    public long AvailableInternalStorage { get; internal set; }

    /// <summary>
    /// If <c>true</c> indicates that the device has a removable storage.
    /// </summary>
    public bool HasRemovableExternalStorage { get; internal set; }

    /// <summary>
    /// If <c>true</c> indicates that the app can write in the removable storage.
    /// </summary>
    public bool CanWriteRemovableExternalStorage { get; internal set; }

    /// <summary>
    /// Total size (in bytes) of the removable external storage.
    /// </summary>
    public long TotalRemovableExternalStorage { get; internal set; }

    /// <summary>
    /// Available size (in bytes) of the removable external storage.
    /// </summary>
    public long AvailableRemovableExternalStorage { get; internal set; }

    /// <summary>
    /// Free size (in bytes) of the removable external storage.
    /// </summary>
    public long FreeRemovableExternalStorage { get; internal set; }
}
