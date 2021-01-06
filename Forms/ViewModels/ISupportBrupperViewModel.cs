using MvvmCross.ViewModels;

namespace Brupper.Forms.ViewModels
{
    // TODO: 
    /// <summary> ISupportBrupperViewModel </summary>
    public interface ISupportBrupperViewModel : IMvxViewModel
    {
        /// <summary>
        /// CanViewDestroy
        /// </summary>
        bool CanViewDestroy { get; set; }
    }
}
