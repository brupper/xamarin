using MvvmCross;
using MvvmCross.Localization;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        #region Fields

        private readonly IMvxTextProvider textProvider;

        #endregion

        #region Constructors

        public TranslateExtension()
        {
            textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>();
        }

        #endregion

        #region Properties

        public string Text { get; set; }

        #endregion

        #region Implementation of IMarkupExtension

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return string.Empty;
            }

            var localizedText = textProvider.GetText(null, null, Text);
            //var translation = Labels.ResourceManager.GetString(Text);
            if (localizedText == null)
            {
#if DEBUG
                throw new ArgumentNullException($"'{Text}' was not found in Recources for culture '{CultureInfo.CurrentUICulture.Name ?? "-"}'");
#else
                localizedText = Text;
#endif
            }

            return localizedText;
        }

        #endregion
    }
}
