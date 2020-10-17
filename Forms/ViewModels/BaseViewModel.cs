using Brupper.Forms;
using Brupper.ViewModels.Popups;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.ViewModels
{
    public abstract class BaseViewModel : MvxNavigationViewModel, IPopupDialogViewModel
    {
        public event EventHandler<string> Alert;

        private bool isRefreshInProgress;
        private bool isBusy;
        private string title = string.Empty;
        private IMvxAsyncCommand refreshCommand;
        private IMvxAsyncCommand backPressedCommand;

        #region Contructors

        public BaseViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
            PermissionHelper = Mvx.IoCProvider.GetSingleton<IPermissionHelper>();
        }

        #endregion

        #region Properties
        public IPermissionHelper PermissionHelper { get; }

        protected internal bool CanViewDestroy { get; set; } = true;

        public bool IsBusy
        {
            get => isBusy;
            set { SetProperty(ref isBusy, value); }
        }

        public string Title
        {
            get => title;
            set { SetProperty(ref title, value); }
        }

        public IMvxAsyncCommand RefreshCommand
            => refreshCommand ?? (refreshCommand = new MvxAsyncCommand(ExecuteRefreshCommand));

        public IMvxAsyncCommand BackPressedCommand
            => backPressedCommand ?? (backPressedCommand = new MvxAsyncCommand(ExecuteBackPressedCommandAsync));

        ICommand IPopupDialogViewModel.BackPressedCommand => this.BackPressedCommand;

        protected virtual Task ExecuteBackPressedCommandAsync()
            => NavigationService.Close(this);

        private async Task ExecuteRefreshCommand()
        {
            if (isRefreshInProgress)
            {
                return;
            }

            try
            {
                isRefreshInProgress = true;
                await InternalExecuteRefreshCommand(true);
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception);
            }
            finally
            {
                isRefreshInProgress = false;
            }
        }

        #endregion

        protected void OnAlert(string message)
            => InvokeOnMainThread(() => Alert?.Invoke(this, message));

        protected virtual Task InternalExecuteRefreshCommand(bool forceRefresh = false)
            => Task.CompletedTask;
    }

    /*
    public abstract class BaseViewModel<TParam> : BaseViewModel, IMvxViewModel<TParam>
    {
        protected BaseViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        public abstract void Prepare(TParam parameter);
    }

    public abstract class BaseViewModel<TParam, TResult> : BaseViewModel, IMvxViewModel<TParam, TResult>
    {
        protected BaseViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (CanViewDestroy && viewFinishing && CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted)
            {
                CloseCompletionSource.TrySetCanceled();
            }

            base.ViewDestroy(viewFinishing);
        }

        public abstract void Prepare(TParam parameter);
    }

    public abstract class ViewModelResultBase<TResult> : BaseViewModel, IMvxViewModelResult<TResult>
    {
        #region Constructors

        public ViewModelResultBase(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        #endregion

        #region Lifecycle Methods

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (CanViewDestroy && viewFinishing && CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted)
            {
                CloseCompletionSource.TrySetCanceled();
            }

            base.ViewDestroy(viewFinishing);
        }

        #endregion
    }
    */
}
