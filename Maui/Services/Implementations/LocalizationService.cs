using System.Globalization;
using System.Resources;

namespace Brupper.Maui.Services.Implementations;

/// <summary>
/// Default implementation of <see cref="ILocalizationService"/>.
/// Provides localization support using .NET resource files (.resx).
/// </summary>
public class LocalizationService : ILocalizationService
{
    private CultureInfo _currentCulture;
    private readonly ResourceManager? _resourceManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class.
    /// </summary>
    /// <param name="resourceManager">Optional resource manager for custom resources.</param>
    public LocalizationService(ResourceManager? resourceManager = null)
    {
        _resourceManager = resourceManager;
        _currentCulture = CultureInfo.CurrentUICulture;
    }

    /// <inheritdoc/>
    public CultureInfo CurrentCulture => _currentCulture;

    /// <inheritdoc/>
    public string GetText(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return key;
        }

        try
        {
            // Try to get from resource manager first
            if (_resourceManager != null)
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            // Fallback: return the key itself if not found
            return key;
        }
        catch
        {
            // If any error occurs, return the key
            return key;
        }
    }

    /// <inheritdoc/>
    public string GetText(string key, params object[] args)
    {
        var text = GetText(key);
        
        if (args == null || args.Length == 0)
        {
            return text;
        }

        try
        {
            return string.Format(_currentCulture, text, args);
        }
        catch
        {
            // If formatting fails, return unformatted text
            return text;
        }
    }

    /// <inheritdoc/>
    public void SetCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        
        _currentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
    }
}
