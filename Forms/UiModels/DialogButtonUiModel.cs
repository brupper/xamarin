using System.Windows.Input;

namespace Brupper.Forms.UiModels
{
    public class DialogButtonUiModel
    {
        public ICommand Command { get; set; }

        public string TranslateKey { get; set; }
    }
}
