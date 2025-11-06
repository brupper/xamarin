using System.Globalization;

namespace Brupper.Maui.Services;

/// <summary>
/// Service for localization and text resource management.
/// Replaces MvvmCross IMvxTextProvider.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the current culture used for localization.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Gets a localized string by its key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string, or the key if not found.</returns>
    string GetText(string key);

    /// <summary>
    /// Gets a localized string by its key with formatting.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string, or the key if not found.</returns>
    string GetText(string key, params object[] args);

    /// <summary>
    /// Sets the current culture for localization.
    /// </summary>
    /// <param name="culture">The culture to set.</param>
    void SetCulture(CultureInfo culture);
}
