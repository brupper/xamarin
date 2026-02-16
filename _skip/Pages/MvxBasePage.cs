using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;

namespace Brupper.Forms.Pages
{
    public class MvxBasePage<TViewModel> : MvxContentPage<TViewModel> where TViewModel : class, IMvxViewModel
    {
        protected override bool OnBackButtonPressed()
        {
            //if (ViewModel != null)//is ViewModels.BaseViewModel baseVm)
            if (ViewModel is Brupper.ViewModels.Abstraction.ViewModelBase baseVm)
            {
                baseVm.BackPressedCommand?.Execute(null);
                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}
