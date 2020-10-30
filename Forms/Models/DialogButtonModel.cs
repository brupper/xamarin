using System.Windows.Input;

namespace Brupper.Forms.Models
{
    public class DialogButtonModel
    {
        public ICommand Command { get; set; }

        public string TranslateKey { get; set; }
    }
}
