using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChipView
    {
        #region TextColorProperty

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty =
                BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChipView), Color.FromHex("#3C5359"));

        #endregion

        #region BorderColorProperty

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty =
                BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ChipView), Color.FromHex("#727272"));

        #endregion

        #region KeyTextProperty

        public string KeyText
        {
            get => (string)GetValue(KeyTextProperty);
            set => SetValue(KeyTextProperty, value);
        }

        public static readonly BindableProperty KeyTextProperty =
                BindableProperty.Create(nameof(KeyText), typeof(string), typeof(ChipView), string.Empty);

        #endregion

        #region ValueTextProperty

        public string ValueText
        {
            get => (string)GetValue(ValueTextProperty);
            set => SetValue(ValueTextProperty, value);
        }

        public static readonly BindableProperty ValueTextProperty =
                BindableProperty.Create(nameof(ValueText), typeof(string), typeof(ChipView), string.Empty);

        #endregion

        public ChipView()
        {
            InitializeComponent();
        }
    }
}