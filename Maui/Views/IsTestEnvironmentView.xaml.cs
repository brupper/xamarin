using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace Brupper.Maui.Views;

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