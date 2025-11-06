using Brupper.Forms.Attributes;
using Brupper.ViewModels.Popups;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Forms.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using Rg.Plugins.Popup.Pages;

namespace Brupper.Forms.Pages.Base
{
    [PopupPresentation(Animated = true, WrapInNavigationPage = false)]
    public partial class MvxPopupPage<TViewModel> : PopupPage, IMvxPage<TViewModel>
           where TViewModel : class, IMvxViewModel, IPopupDialogViewModel
    {
        private IMvxBindingContext bindingContext;

        public MvxPopupPage()
        {
            CloseWhenBackgroundIsClicked = false;
            HasSystemPadding = false;
        }

        public TViewModel ViewModel
        {
            get => (TViewModel)((IMvxView)this).ViewModel;
            set => ((IMvxView)this).ViewModel = value;
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

        protected virtual void OnViewModelSet()
        {
        }

        public MvxFluentBindingDescriptionSet<IMvxElement<TViewModel>, TViewModel> CreateBindingSet()
            => this.CreateBindingSet<IMvxElement<TViewModel>, TViewModel>();

        #region Overrided Methods

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
            if (ViewModel != null)//is ViewModels.BaseViewModel baseVm)
            {
                ViewModel.BackPressedCommand?.Execute(null);
                return true;
            }

            return base.OnBackButtonPressed();
        }

        protected override bool OnBackgroundClicked()
        {
            if (CloseWhenBackgroundIsClicked)
            {
                ViewModel?.BackPressedCommand?.Execute(null);
            }

            return base.OnBackgroundClicked();
        }

        #endregion
    }
}
