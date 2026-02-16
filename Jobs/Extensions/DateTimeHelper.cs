
using System;
using System.Globalization;

internal static class DateTimeHelper
{
    private static string RoundTripFormatString = "o";

    /// <summary>
    /// Converts a DateTime instance into string using the "round-trip" format.
    /// This format preserves accuracy up to the ten millionth of a second, as well
    /// as time zone information.
    /// </summary>
    /// <param name="dateTime">The DateTime instance to transform.</param>
    /// <returns>A culture-invariant string representing the given DateTime instance in
    /// "round-trip" format.</returns>
    public static string ToRoundTrip(DateTime dateTime) => dateTime.ToString(RoundTripFormatString, CultureInfo.InvariantCulture);

    /// <summary>
    /// Converts a string into a DateTime instance using the "round-trip" format.
    /// </summary>
    /// <param name="dateTime">The string to parse.</param>
    /// <returns>A DateTime instance matching the provided string.</returns>
    public static DateTime FromRoundTrip(string dateTime) => DateTime.ParseExact(dateTime, RoundTripFormatString, CultureInfo.InvariantCulture);
}
