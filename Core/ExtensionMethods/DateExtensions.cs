using System;

public static class DateExtensions
{
	/// <summary>
	/// Seconds
	/// </summary>
	public static double DateTimeToUnixTimestamp(this DateTime dateTime, bool useMilliseconds = false)
	{
		if (dateTime.Kind != DateTimeKind.Utc && dateTime > DateTime.MinValue && dateTime < DateTime.MaxValue)
		{
			dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
		}
		var dt = (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
		return (double) (useMilliseconds ? dt.TotalMilliseconds : dt.TotalSeconds);
	}

	public static string DateTimeToUnixTimestampString(this DateTime dateTime, bool useMilliseconds = false)
	{
		var d = dateTime.DateTimeToUnixTimestamp(useMilliseconds);
		return Convert.ToInt64(d).ToString();
	}

	public static DateTime UnixTimeStampToDateTime(this string unixTimeStamp, bool useMilliseconds = false)
	{
		double t;
		if (double.TryParse(unixTimeStamp, out t))
		{
			return t.UnixTimeStampToDateTime(useMilliseconds);
		}
		return DateTime.MinValue;
	}

	public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp, bool useMilliseconds = false)
	{
		// Unix timestamp is seconds past epoch
		var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		dtDateTime = (useMilliseconds ? dtDateTime.AddMilliseconds(unixTimeStamp) : dtDateTime.AddSeconds(unixTimeStamp));
		return dtDateTime;
	}

	public static DateTime? UnixTimeStampToDateTime(this long unixTimeStamp, bool useMilliseconds = false)
	{
		if (unixTimeStamp == 0) return null;
		// Unix timestamp is seconds past epoch
		DateTime? dtDateTime = new DateTime();
		dtDateTime = (useMilliseconds ? dtDateTime.Value.AddMilliseconds(unixTimeStamp) : dtDateTime.Value.AddSeconds(unixTimeStamp));
		return dtDateTime;
	}
}
