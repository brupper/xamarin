using System;

internal static class IProgressExtensions
{
    /// <summary>
    /// Reports a progress update, checking first that the <see cref="T:System.IProgress{T}"/> instance
    /// is not <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    /// <param name="this">A reference to an instance of <see cref="T:System.IProgress{T}"/>.</param>
    /// <param name="value">The value of the updated progress.</param>
    public static void ReportProgress<T>(this IProgress<T> @this, T value)
    {
        @this?.Report(value);
    }
}
