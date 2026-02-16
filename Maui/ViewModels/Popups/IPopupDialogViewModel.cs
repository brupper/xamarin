using System.Windows.Input;

namespace Brupper.ViewModels.Popups;

/// <summary> . </summary>
public interface IPopupDialogViewModel
{
    ICommand BackPressedCommand { get; }
}
