using Brupper.Forms.Services.Interfaces;
using Brupper.ViewModels.Popups;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace Brupper.ViewModels
{
    public abstract partial class ViewModelBase : MvxNavigationViewModel, IPopupDialogViewModel
    {
        #region Fields

        public event EventHandler<string> Alert;

        private bool isBusy;
        private string message;
        private string syncMessage;
        private IMvxTextProvider textProvider;
        private IMvxAsyncCommand backPressedCommand;

        #endregion

        #region Constructors

        public ViewModelBase(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
            Logger = logProvider.GetLogFor<AppCenterTrace>();

        }

        #endregion

        #region Properties

        public abstract string Name { get; }

        /// <summary> https://www.cazzulino.com/git-info-from-msbuild-and-code.html </summary>
        public string Version => $"{AppInfo.VersionString} ({AppInfo.BuildString})";

        protected IMvxLog Logger { get; private set; }

        public IMvxTextProvider TextProvider
                => textProvider ?? (textProvider = Mvx.IoCProvider.GetSingleton<IMvxTextProvider>());

        public IPlatformInformationService PlatformInformationService
        {
            get
            {
                Mvx.IoCProvider.TryResolve<IPlatformInformationService>(out var service);
                return service;
            }
        }

        protected internal bool CanViewDestroy { get; set; } = true;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }

        public string SyncMessage
        {
            get => syncMessage;
            set => SetProperty(ref syncMessage, value);
        }

        public IMvxAsyncCommand BackPressedCommand
            => backPressedCommand ?? (backPressedCommand = new MvxAsyncCommand(ExecuteBackPressedCommandAsync));

        ICommand IPopupDialogViewModel.BackPressedCommand => BackPressedCommand;

        #endregion

        #region Overrides

        public override Task Initialize()
        {
            Logger.TrackEvent($"Initializing: {Name}");
            return base.Initialize();
        }

        #endregion

        #region Protected Methods

        protected async Task InvokeOnMainThreadAsync(Task action, bool maskExceptions = true)
        {
            var tcs = new TaskCompletionSource<bool>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    await action;
                    tcs.TrySetResult(true);
                }
                catch (Exception e)
                {
                    if (!maskExceptions)
                        tcs.TrySetException(e);
                    else
                        tcs.TrySetResult(false);
                }
            });

            await tcs.Task;
        }

        protected virtual Task ExecuteBackPressedCommandAsync()
            => NavigationService.Close(this);

        #region MvxMessenger message handling

        #endregion

        protected virtual void OnAlert(string message)
        {
            try { Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => Alert?.Invoke(this, message)); }
            catch (PlatformNotSupportedException) { }
        }

        public bool CheckInternet(bool showAlert = true)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                if (showAlert) OnAlert(TextProvider.GetText(null, null, "general_nointernet"));
                return false;
            }

            return true;
        }

        #endregion
    }

    public abstract class ViewModelBase<TParam> : ViewModelBase, IMvxViewModel<TParam>
    {
        #region Constructors

        public ViewModelBase(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Lifecycle Methods

        public abstract void Prepare(TParam parameter);

        #endregion
    }

    public abstract class ViewModelBase<TParam, TResult> : ViewModelBase, IMvxViewModel<TParam, TResult>
    {
        #region Constructors

        public ViewModelBase(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        #endregion

        #region Lifecycle Methods

        public abstract void Prepare(TParam parameter);

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

    public abstract class ViewModelResultBase<TResult> : ViewModelBase, IMvxViewModelResult<TResult>
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
}
