using Brupper.ViewModels.Popups;
using MvvmCross.ViewModels;
using Xamarin.Forms.Xaml;

namespace Brupper.Forms.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [MvxViewFor(typeof(AlertViewModel))]
    public partial class AlertPage
    {
        public AlertPage()
            => InitializeComponent();

    }
}