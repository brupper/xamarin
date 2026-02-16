using System;
using System.Text.Json.Serialization;

namespace Brupper.Jobs.FileTransfer.Azure;

public class SasResponse
{
    #region Helpers

    // [JsonIgnore] public DateTime Expiration => CreatedAt.AddSeconds(Ttl).AddSeconds(-10);
    // [JsonIgnore] public TimeSpan Validity => Expiration - DateTime.Now;

    /// <summary>  </summary>
    [JsonIgnore]
    public DateTime CreatedAt { get; } = DateTime.Now;

    /// <summary> Is SAS token expired? </summary>
    /// <remarks>
    /// We calucalte this in timespan. This works fine even if the device internal clock is not set properly.
    /// This will return expired a bit sooner, as this instaces created time and token created time could be different (due to network issues)
    /// </remarks>
    [JsonIgnore]
    public bool IsExpired => CreatedAt + new TimeSpan(0, 0, Ttl) < DateTime.Now;

    #endregion

    /// <summary> SAS token </summary>
    public string Token { get; set; }

    /// <summary> SAS Token time to live in seconds </summary>
    public int Ttl { get; set; }
}
