using Microsoft.AppCenter.Crashes;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Abstraction
{
    public abstract class AItemsSearchViewModel<TEntity> : ViewModelBase
        where TEntity : class, new()
    {
        #region Fields

        protected readonly List<TEntity> cachedEntities = new List<TEntity>();
        protected readonly List<TEntity> filteredEntities = new List<TEntity>();

        protected CancellationTokenSource searchCancellationTokenSource = new CancellationTokenSource();

        private int currentPage;
        private MvxObservableCollection<TEntity> filteredItems = new MvxObservableCollection<TEntity>(); // Xamarin Bug: obsarvable collections can not be null by default
        private string filterText;
        private TEntity selectedCustomer;
        private IMvxAsyncCommand<string> searchCommand;
        private MvxAsyncCommand refreshCommand;


        #endregion

        #region Constructor

        protected AItemsSearchViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        protected virtual string IncludeProperties { get; } = "";

        /// <summary> Should set to: int.MaxValue </summary>
        protected virtual int PageSize { get; } = int.MaxValue;

        public int CurrentPage
        {
            get => currentPage;
            set => SetProperty(ref currentPage, value);
        }

        public TEntity SelectedItem
        {
            get => selectedCustomer;
            set => SetProperty(ref selectedCustomer, value);
        }

        /// <summary> Items to be bind to the UI </summary>
        public MvxObservableCollection<TEntity> FilteredItems
        {
            get => filteredItems;
            set
            {
                SetProperty(ref filteredItems, value);
                RefreshButtonStates();
            }
        }

        public string FilterText
        {
            get => filterText;
            set
            {
                SetProperty(ref filterText, value);
                _ = ExecuteSearchCommandAsync(FilterText);
            }
        }

        public IMvxAsyncCommand<string> SearchCommand
            => searchCommand ?? (searchCommand = new MvxAsyncCommand<string>(ExecuteSearchCommandAsync));

        public IMvxAsyncCommand RefreshCommand => refreshCommand ?? (refreshCommand = new MvxAsyncCommand(ExecuteRefreshCommand));

        #endregion

        #region Overrides

        public override Task Initialize()
        {
            // make sure its getting run on backgroundthread
            ThreadPool.QueueUserWorkItem(_ =>
            {
                _ = ExecuteRefreshCommand().ContinueWith(t =>
                {
                    FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
                }).ConfigureAwait(false);
            });

            return base.Initialize();
        }

        #endregion

        #region Command execution

        protected virtual async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
            {
                // TODO: return; // a PullToRefresh is ezt hivogatja, ha pl popupot jelenitunk meg....
            }

            IsBusy = true;

            int allItemCount = cachedEntities.Count;

            await ReloadAsync().ConfigureAwait(false);

            if (allItemCount != cachedEntities.Count)
            {
                CurrentPage = 0;
            }

            if (filteredEntities.Any())
            {
                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize));
            }
            else
            {
                FilteredItems = new MvxObservableCollection<TEntity>();
            }

            IsBusy = false;
        }

        protected virtual async Task ExecuteSearchCommandAsync(string filterTextParam)
        {
            if (string.IsNullOrEmpty(filterTextParam))
            {
                filteredEntities.Clear();
                filteredEntities.AddRange(cachedEntities);

                CurrentPage = 0;
                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
                return;
            }

            if (filterTextParam?.Length < 3)
            {
                return;
            }

            searchCancellationTokenSource?.Cancel();
            searchCancellationTokenSource = new CancellationTokenSource();
            var searchCancellationToken = searchCancellationTokenSource.Token;

            try
            {
                var entities = await InternalFilterAsync(searchCancellationToken, filterTextParam);
                filteredEntities.Clear();
                filteredEntities.AddRange(entities);

                CurrentPage = 0;
                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                Crashes.TrackError(exception);
            }
            finally
            {
                RefreshButtonStates();
            }
        }

        #endregion

        protected virtual Task ReloadAsync()
        {
            try
            {
                filteredEntities.Clear();
                filteredEntities.AddRange(cachedEntities);
            }
            catch (Exception exception)
            {
                cachedEntities.Clear();
                filteredEntities.Clear();

                Logger?.TraceException(exception.Message, exception);
            }
            finally
            {
                RefreshButtonStates();
            }

            return Task.CompletedTask;
        }

        protected abstract Task<IEnumerable<TEntity>> InternalFilterAsync(CancellationToken searchCancellationToken, string filterTextParam);

        protected virtual void RefreshButtonStates() { }
    }
}
