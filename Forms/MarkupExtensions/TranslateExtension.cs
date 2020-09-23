using MvvmCross;
using MvvmCross.Localization;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private readonly IMvxTextProvider textProvider;

        public string Text { get; set; }

        public TranslateExtension()
        {
            textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return string.Empty;
            }

            var translation = textProvider.GetText(null, null, Text);
            //var translation = Labels.ResourceManager.GetString(Text);
            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{Text}' was not found in resources for culture '{(CultureInfo.CurrentUICulture.Name ?? "<???>")}'.");
#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }

            return translation;
        }
    }
}
