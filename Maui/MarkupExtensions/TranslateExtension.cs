using Brupper.Localization;
using Brupper.Maui.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Brupper.Maui;

[ContentProperty("Text")]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    #region Fields

    private readonly ITextProvider textProvider;

    #endregion

    #region Constructors

    public TranslateExtension()
    {            
        textProvider = ServiceProvider.GetService<ITextProvider>();
        // try { } catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"{ex}"); throw; }
    }

    #endregion

    #region Properties

    public string Text { get; set; }

    #endregion

    #region Implementation of IMarkupExtension

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Text}]",
            Source = this,
        };

        return binding;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }

    #endregion

    public string this[string text]
    {
        get
        {
            var localizedText = textProvider.GetText(null, null, Text);
            //var translation = Labels.ResourceManager.GetString(Text);
            if (localizedText == null)
            {
#if DEBUG
                var errotMessage = $"'{Text}' was not found in Recources for culture '{System.Globalization.CultureInfo.CurrentUICulture.Name ?? "-"}'";
                System.Diagnostics.Debug.WriteLine(errotMessage);
                // throw new ArgumentNullException(errotMessage);
#else
                localizedText = Text;
#endif
            }

            return localizedText;
        }
    }
}
