using System.Windows.Input;

namespace Brupper.Maui.UiModels;

public class DialogButtonUiModel
{
    public bool ShouldClosePopup { get; set; } = true;

    public ICommand Command { get; set; }

    public string TranslateKey { get; set; }

    public bool Result { get; set; }
}
