using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IsTestEnvironmentView
    {
        public IsTestEnvironmentView() => InitializeComponent();

        #region TextProperty

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty =
                BindableProperty.Create(nameof(Text), typeof(string), typeof(IsTestEnvironmentView), "TEST");

        #endregion

    }
}