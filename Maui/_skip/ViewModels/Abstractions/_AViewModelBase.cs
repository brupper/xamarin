using Brupper.Maui;
using Brupper.Maui.Services;
using Brupper.Maui.UiModels;
using Brupper.Maui.ViewModels;
using Brupper.ViewModels.Popups;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using MvvmCross;
using MvvmCross.Localization;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.ViewModels.Abstraction;

/*ObservableObject,*/
public abstract partial class ViewModelBase : MvxViewModel, IPopupDialogViewModel, ISupportBrupperViewModel

{
    #region Fields

    public event EventHandler<string> Alert;

    private int isBusyCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncMessage))]
    private string message;

    [ObservableProperty]
    private string syncMessage;

    private ITextProvider textProvider;

    #endregion

    #region Constructors

    public ViewModelBase(ILoggerFactory logProvider, IMvxNavigationService navigationService)
        : base()
    {
    }

    #endregion

    #region Properties

    public abstract string Name { get; }

    /// <summary> https://www.cazzulino.com/git-info-from-msbuild-and-code.html </summary>
    public string Version => $"{AppInfo.VersionString} ({AppInfo.BuildString})";

    protected ILogger Logger => base.Log;

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

    public virtual bool CanViewDestroy { get; set; } = true;

    public bool IsBusy
    {
        get => isBusyCount > 0;
        set => SetProperty(ref isBusyCount, isBusyCount + (value ? 1 : -1));
    }

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

    [RelayCommand]
    protected virtual Task BackPressedAsync()
    {
        return this.Close(this);
    }

    #region MvxMessenger message handling

    #endregion

    protected virtual void OnAlert(string message)
    {
        try { MainThread.BeginInvokeOnMainThread(() => Alert?.Invoke(this, message)); }
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

    public virtual Task ShowAlert(string message, bool error = true)
        => NavigationService.Navigate<QuestionViewModel, QuestionViewModelParam, DialogButtonUiModel>(new QuestionViewModelParam
        {
            LocalizedQuestion = $"{TextProvider.GetText(null, null, "general_error_header")}\n\n{message}",
            CancelButton = null
        });

    public virtual Task ShowAlertWithKey(string key = null, bool error = true)
        => ShowAlert(TextProvider.GetText(null, null, key ?? "general_error"), error);

    #endregion
}

public abstract class ViewModelBase<TParam> : ViewModelBase, IMvxViewModel<TParam>
{
    #region Constructors

    public ViewModelBase(ILoggerFactory logProvider, IMvxNavigationService navigationService)
        : base(logProvider, navigationService)
    { }

    #endregion

    #region Lifecycle Methods

    public abstract void Prepare(TParam parameter);

    #endregion
}

public abstract class ViewModelBase<TParam, TResult> : ViewModelBase, IMvxViewModel<TParam, TResult>
    where TResult : class
{
    #region Constructors

    public ViewModelBase(ILoggerFactory logProvider, IMvxNavigationService navigationService)
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
    where TResult : class
{
    #region Constructors

    public ViewModelResultBase(ILoggerFactory logProvider, IMvxNavigationService navigationService)
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
