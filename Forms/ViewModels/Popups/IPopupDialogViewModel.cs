using System.Windows.Input;

namespace Brupper.ViewModels.Popups
{
    public interface IPopupDialogViewModel
    {
        ICommand BackPressedCommand { get; }
    }
}
