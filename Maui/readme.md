Before your start:

Experimantal FLAGS we do use:
    - Brush_Experimental
    - SwipeView_Experimental
    - CarouselView_Experimental
    - IndicatorView_Experimental  // https://devblogs.microsoft.com/xamarin/xamarin-forms-4-4/

Initialize Rg.Plugins.Popup:
- Android: 
    OnCreate(Bundle savedInstanceState) => Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
    OnBackPressed() => if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed)) ... ;
- iOS:
     Setup.cs => Rg.Plugins.Popup.Popup.Init(); 
     !!! <PackageReference Include="Rg.Plugins.Popup" Version="2.0.0.10" />

Initialize FFImageLoading:
If you reference Brupper.Forms in your platform projects (iOS&Android)=> it automatically initialize FFImageLoading ;)
//- Android: 
//    FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
//- iOS:
//    Setup.cs => FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

Initialize ZXing:
- just kidding, we do not use ZXing... YET! HAHAAHA


Create your own VMs:



    public abstract class BaseViewModel<TParam> : BaseViewModel, IMvxViewModel<TParam>
    {
        protected BaseViewModel(
            ILoggerFactory logProvider,
            INavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        public abstract void Prepare(TParam parameter);
    }

    public abstract class BaseViewModel<TParam, TResult> : BaseViewModel, IMvxViewModel<TParam, TResult>
    {
        protected BaseViewModel(
            ILoggerFactory logProvider,
            INavigationService navigationService)
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

        public ViewModelResultBase(ILoggerFactory logProvider, INavigationService navigationService)
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
