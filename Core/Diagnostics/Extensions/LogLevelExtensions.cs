// ReSharper disable once CheckNamespace
namespace Brupper.Diagnostics
{
    public static class LogLevelExtensions
    {
        public static LogTag LogLevelToLogTag(this LogLevel loglevel)
        {
            switch (loglevel)
            {
                case LogLevel.Undefined:
                    return LogTagLevels.Medium;
                case LogLevel.High:
                    return LogTagLevels.High;
                case LogLevel.Medium:
                    return LogTagLevels.Medium;
                case LogLevel.Low:
                    return LogTagLevels.Low;
                case LogLevel.None:
                    return LogTag.None;
                default:
                    return LogTagLevels.All;

            }
        }
    }
}
