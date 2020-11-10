using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Abstraction
{
    public class SelectorParam<T>
    {
        public T Subject { get; set; }

        public IEnumerable<T> Items { get; set; }
            = Enumerable.Empty<T>();
    }

    public abstract class ASelectorViewModel<TSubject> : AItemsSearchViewModel<TSubject>, IMvxViewModel<SelectorParam<TSubject>, TSubject>
        where TSubject : class, INotifyPropertyChanged, new()
    {
        #region Fields

        protected SelectorParam<TSubject> parameter;

        private IMvxAsyncCommand acceptCommand;
        private IMvxCommand<TSubject> selectExistingCommand;

        #endregion

        #region Constructor

        protected ASelectorViewModel(
            IMvxLogProvider logProvider
            , IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        public TaskCompletionSource<object> CloseCompletionSource { get; set; }

        public IMvxAsyncCommand AcceptCommand => acceptCommand ?? (acceptCommand = new MvxAsyncCommand(ExecuteAcceptCommand));

        public IMvxCommand<TSubject> SelectExistingCommand => selectExistingCommand ?? (selectExistingCommand = new MvxCommand<TSubject>(ExecuteSelectExistingCommand));

        #endregion

        #region Execute commands

        protected virtual Task ExecuteAcceptCommand() => NavigationService.Close(this, SelectedItem);

        protected virtual void ExecuteSelectExistingCommand(TSubject arg)
        {
            if (arg != null)
            {
                SelectedItem = arg;
            }
        }

        #endregion

        #region Overrides

        public override void ViewDestroy(bool viewFinishing = true)
        {
            if (CanViewDestroy && viewFinishing && CloseCompletionSource != null && !CloseCompletionSource.Task.IsCompleted && !CloseCompletionSource.Task.IsFaulted)
            {
                CloseCompletionSource.TrySetCanceled();
            }

            base.ViewDestroy(viewFinishing);
        }

        public virtual void Prepare(SelectorParam<TSubject> parameter)
        {
            this.parameter = parameter;
            SelectedItem = parameter.Subject ?? throw new ArgumentNullException($"{parameter.Subject} cannot be null!"); // make sure it subscribes the changed event
            cachedEntities.AddRange(parameter?.Items ?? Enumerable.Empty<TSubject>());
        }

        protected override Task ExecuteBackPressedCommandAsync() => NavigationService.Close(this, SelectedItem);

        #endregion

        protected abstract void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}
