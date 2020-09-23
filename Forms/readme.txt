Create your own VMs:



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
