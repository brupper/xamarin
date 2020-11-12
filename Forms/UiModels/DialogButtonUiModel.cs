using System.Windows.Input;

namespace Brupper.Forms.UiModels
{
    public class DialogButtonUiModel
    {
        public bool ShouldClosePopup { get; set; } = true;

        public ICommand Command { get; set; }

        public string TranslateKey { get; set; }
    }
}
