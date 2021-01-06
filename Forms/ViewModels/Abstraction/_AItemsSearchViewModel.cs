using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
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
        private TEntity selected;
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

        public virtual TEntity SelectedItem
        {
            get => selected;
            set => SetProperty(ref selected, value);
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
                _ = ExecuteSearchCommandAsync(FilterText).ConfigureAwait(false);
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
            try
            {
                if (IsBusy)
                {
                    // TODO: return; // a PullToRefresh is ezt hivogatja, ha pl popupot jelenitunk meg....
                }

                IsBusy = true;
                int allItemCount = filteredEntities.Count;
                await ReloadAsync().ConfigureAwait(false);

                await FilterEntities();

                if (allItemCount != filteredEntities.Count)
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
            }
            catch (Exception exception)
            {
                Logger.TrackError(exception);
                await ShowAlertWithKey("general_error_search");
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ExecuteSearchCommandAsync(string filterTextParam)
        {
            try
            {
                IsBusy = true;

                await FilterEntities();

                CurrentPage = 0;
                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
            }
            catch (Exception exception)
            {
                Logger.TrackError(exception);
                await ShowAlertWithKey("general_error_list_refresh");
            }
            finally
            {
                IsBusy = false;
                RefreshButtonStates();
            }
        }

        #endregion

        protected override async Task ReloadAsync()
        {
            try
            {
                IsBusy = true;
                filteredEntities.Clear();
                filteredEntities.AddRange(cachedEntities);
            }
            catch (Exception exception)
            {
                cachedEntities.Clear();
                filteredEntities.Clear();
                Logger.TrackError(exception);
                await ShowAlertWithKey(null);
            }
            finally
            {
                IsBusy = false;
                RefreshButtonStates();
            }
        }

        protected abstract Task<IEnumerable<TEntity>> InternalFilterAsync(CancellationToken searchCancellationToken, string filterTextParam);

        protected virtual void RefreshButtonStates() { }

        private async Task FilterEntities()
        {
            if (FilterText?.Length >= 3)
            {
                searchCancellationTokenSource?.Cancel();
                searchCancellationTokenSource = new CancellationTokenSource();
                var searchCancellationToken = searchCancellationTokenSource.Token;

                var entities = await InternalFilterAsync(searchCancellationToken, FilterText);
                filteredEntities.Clear();
                filteredEntities.AddRange(entities);
            }
            else
            {
                filteredEntities.Clear();
                filteredEntities.AddRange(cachedEntities);
            }

        }
    }
}
