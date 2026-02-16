using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    /// <summary> Provides methods to start and stop the timing for a process. </summary>
    public static class TrackTimeHandler
    {
        private static readonly IDictionary<string, DateTime> ProcessStartDates = new Dictionary<string, DateTime>();

        private static readonly IDictionary<string, DateTime> SubProcessStartDates = new Dictionary<string, DateTime>();

        public static void Start(string key, string? processId = null)
        {
            if (processId != null)
            {
                SubProcessStartDates[processId] = DateTime.UtcNow;
            }
            else
            {
                ProcessStartDates[key] = DateTime.UtcNow;
            }
        }

        public static TimeSpan Stop(string key, string? processId = null)
        {
            DateTime startTime;

            if (processId != null)
            {
                if (SubProcessStartDates.TryGetValue(processId, out startTime))
                {
                    return (DateTime.UtcNow - startTime).Duration();
                }
            }
            else
            {
                if (ProcessStartDates.TryGetValue(key, out startTime))
                {
                    return (DateTime.UtcNow - startTime).Duration();
                }
            }

            return TimeSpan.Zero;
        }

        public static void Reset()
        {
            ProcessStartDates.Clear();
            SubProcessStartDates.Clear();
        }
    }
}
