using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using Brupper.Forms.Attributes;
using Rg.Plugins.Popup.Pages;
using System.Windows.Input;

namespace Brupper.Forms.Pages.Base
{
    public interface IPopupDialogViewModel
    {
        ICommand BackCommand { get; }
    }

    [PopupPresentation(Animated = true, WrapInNavigationPage = false)]
    public class MvxPopupPage<TViewModel> : PopupPage, IMvxPage<TViewModel>
        where TViewModel : class, IMvxViewModel, IPopupDialogViewModel
    {
        private IMvxBindingContext bindingContext;

        public TViewModel ViewModel
        {
            get => (TViewModel)((IMvxView)this).ViewModel;
            set => ((IMvxView)this).ViewModel = value;
        }

        public MvxPopupPage()
        {
            CloseWhenBackgroundIsClicked = false;
        }

        public object DataContext
        {
            get => BindingContext.DataContext;
            set
            {
                base.BindingContext = value;
                BindingContext.DataContext = value;
            }
        }

        public new IMvxBindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                    BindingContext = new MvxBindingContext(base.BindingContext);
                return bindingContext;
            }
            set => bindingContext = value;
        }

        IMvxViewModel IMvxView.ViewModel
        {
            get => DataContext as IMvxViewModel;
            set
            {
                DataContext = value;
                OnViewModelSet();
            }
        }

        public MvxFluentBindingDescriptionSet<IMvxElement<TViewModel>, TViewModel> CreateBindingSet()
        {
            return this.CreateBindingSet<IMvxElement<TViewModel>, TViewModel>();
        }

        protected virtual void OnViewModelSet()
        {
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.ViewAppearing();
            ViewModel?.ViewAppeared();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.ViewDisappearing();
            ViewModel?.ViewDisappeared();
        }

        protected override bool OnBackButtonPressed()
        {
            ViewModel?.BackCommand?.Execute(null);

            return true;
        }
    }
}
