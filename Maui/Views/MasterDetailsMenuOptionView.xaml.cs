using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MasterDetailsMenuOptionView
{
    #region TappedCommandProperty

    public ICommand TappedCommand
    {
        get => (ICommand)GetValue(TappedCommandProperty);
        set => SetValue(TappedCommandProperty, value);
    }

    public static readonly BindableProperty TappedCommandProperty =
        BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(MasterDetailsMenuOptionView));

    #endregion


    public MasterDetailsMenuOptionView()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(MasterDetailsMenuOptionView), string.Empty);

    public ImageSource Source
    {
        set
        {
            Icon.Source = value;
            Icon.IsVisible = Icon.Source != null;
        }
    }

    private void Handle_Tapped(object sender, EventArgs e)
    {
        // szaggat a masterdetail menü bezárása...
        _ = Task.Delay(350).ContinueWith(t =>
        {
            TappedCommand?.Execute(BindingContext);
        }).ConfigureAwait(false);

        this.CloseMenu();
    }
}