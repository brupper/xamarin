using System;

namespace Brupper
{
    /// <summary> . </summary>
    public static class DateExtensions
    {
        /// <summary> . </summary>
        public static string DateTimeToYearMonthDateFormat(this DateTime dateTime)
            => dateTime.ToString(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);

        /// <summary> . </summary>
        public static DateTime DateToFirstDay(this DateTime input)
            => new DateTime(input.Year, input.Month, 1);
    }

    public class Period
    {
        public static DateTime Now => DateTime.Now;

        public static Period Default => (from: Now.DateToFirstDay(), to: Now.DateToFirstDay().AddMonths(1).AddSeconds(-1) /* Last Day of Month 23:59:59 */);

        public Period(DateTime @from, DateTime to)
        {
            From = @from;
            To = to;
        }

        public Period()
        {
            From = Default.From;
            To = Default.To;
        }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public static implicit operator Tuple<DateTime, DateTime>(Period period)
        {
            return new Tuple<DateTime, DateTime>(period.From, period.To);
        }

        public static implicit operator (DateTime start, DateTime end)(Period period)
        {
            return (start: period.From, end: period.To);
        }

        public static implicit operator Period(Tuple<DateTime, DateTime> period)
        {
            return new Period(period.Item1, period.Item2);
        }

        public static implicit operator Period((DateTime from, DateTime to) period)
        {
            return new Period(period.from, period.to);
        }
    }

}
