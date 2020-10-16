using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Brupper.Forms.Pages.Base;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.ViewModels.Popups
{
    public abstract class MvxPopupViewModel : MvxViewModel, IPopupDialogViewModel
    {
        protected readonly IMvxNavigationService navigationService;

        #region Constructors

        protected MvxPopupViewModel(IMvxNavigationService navigationService)
        {
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(ExecuteBackCommandAsync);
        }

        #endregion

        #region Properties

        public IMvxAsyncCommand BackCommand { get; }

        public IMvxAsyncCommand BackPressedCommand
            => BackCommand;

        ICommand IPopupDialogViewModel.BackPressedCommand
            => BackCommand;

        #endregion

        #region Methods

        protected virtual Task ExecuteBackCommandAsync() => navigationService.Close(this);

        #endregion
    }

    public abstract class MvxPopupViewModel<TParam> : MvxPopupViewModel, IMvxViewModel<TParam>
    {
        protected MvxPopupViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        public abstract void Prepare(TParam parameter);
    }

    public abstract class MvxPopupViewModel<TParam, TResult> : MvxPopupViewModel, IMvxViewModel<TParam, TResult>
    {
        protected MvxPopupViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (viewFinishing && CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted)
                CloseCompletionSource.TrySetCanceled();

            base.ViewDestroy(viewFinishing);
        }

        public abstract void Prepare(TParam parameter);
    }
}
