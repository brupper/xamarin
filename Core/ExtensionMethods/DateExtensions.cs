using System;
using System.Globalization;

namespace Brupper;

/// <summary> . </summary>
public static class DateExtensions
{
    /// <summary> . </summary>
    public static string DateTimeToYearMonthDateFormat(this DateTime dateTime)
        => dateTime.ToString(CultureInfo.InvariantCulture.DateTimeFormat);

    /// <summary> . </summary>
    public static DateTime DateToFirstDay(this DateTime input)
        => new DateTime(input.Year, input.Month, 1);

    // This presumes that weeks start with Monday.
    // Week 1 is the 1st week of the year with a Thursday in it.
    public static int GetIso8601WeekOfYear(DateTime time)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            time = time.AddDays(3);
        }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    //public static (DateTime, DateTime) WeekToDateRange(this int week, int? year = null)
    //{
    //	var start = System.Globalization.ISOWeek.ToDateTime(year ?? DateTime.Now.Year, week, DayOfWeek.Monday);
    //	var end = start.AddMilliseconds(604799999);
    //	return (start, end);
    //}

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
        return (double)(useMilliseconds ? dt.TotalMilliseconds : dt.TotalSeconds);
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

    /// <summary>
    /// The first extension converts a DateTime data type to text, but mentions of how long (days, months, years) the date has been in relation to the current date.This extension is especially useful when working with a news feed or a list of articles, and you want to show the user when the article was published.
    /// </summary>
    /// <returns> "Today", "Yesterday", "DayOfWeek", "One month ago", "N days ago", "1 year ago", "N years ago"   </returns>
    public static string DateTimeToString(this DateTime value)
    {
        if (value == null)
            return string.Empty;
        if (value.Date == DateTime.Now.Date)
            return "Today";
        var numDays = (DateTime.Now.Date - value.Date).Days;
        if (numDays > 0 && numDays < 8)
        {
            if (numDays == 1)
                return "Yesterday";
            else
                return value.DayOfWeek.ToString();
        }
        if (numDays > 8 && numDays < 30)
        {
            return "One month ago";
        }
        if (numDays < 365)
        {
            return numDays + " days ago";
        }
        else
        {
            int years = numDays / 365;
            return (years == 1) ? years + " year ago" : years + " years ago";
        }
    }

    /// <summary> The second extension is very similar to the first, but in this case it only returns in text if it was ‘Today’ or ‘Yesterday’, otherwise it will return the date in text. This extension is often used in the same way in news feeds, but it is a bit more simple, defined and minimalist for the end user. </summary>
    /// <returns> "Today", "Yesterday", "DayOfWeek", short date string </returns>
    public static string DateTimeToString2(this DateTime value)
    {
        if (value == null)
            return string.Empty;
        if (value.Date == DateTime.Now.Date)
            return "Today";
        var numDays = (DateTime.Now.Date - value.Date).Days;
        if (numDays > 0 && numDays < 8)
        {
            if (numDays == 1)
                return "Yesterday";
            else
                return value.DayOfWeek.ToString();
        }
        return value.ToShortDateString().ToUpper();
    }
}
