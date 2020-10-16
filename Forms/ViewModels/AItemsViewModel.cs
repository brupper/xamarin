//using Brupper;
//using Brupper.Data;
//using InspEx.Data.Models;
//using Microsoft.AppCenter.Crashes;
//using MvvmCross;
//using MvvmCross.Commands;
//using MvvmCross.Logging;
//using MvvmCross.Navigation;
//using MvvmCross.ViewModels;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace InspEx.UI.ViewModels
//{
//    public abstract class AItemsViewModel<TEntity, TRepository, TEditViewModel> : ViewModelBase
//        where TEntity : BaseEntity, new()
//        where TRepository : class, IRepository<TEntity>, IFilterableRepository<TEntity>
//        where TEditViewModel : AEditorViewModel<TEntity, TRepository>
//    {
//        #region Fields

//        protected readonly List<TEntity> cachedEntities = new List<TEntity>();
//        protected readonly List<TEntity> filteredEntities = new List<TEntity>();

//        protected CancellationTokenSource searchCancellationTokenSource = new CancellationTokenSource();

//        private MvxObservableCollection<TEntity> filteredItems = new MvxObservableCollection<TEntity>(); // Xamarin Bug: obsarvable collections can not be null by default
//        private int currentPage;
//        private string filterText;
//        private TEntity selectedCustomer;
//        private IMvxAsyncCommand<string> searchCommand;
//        private IMvxAsyncCommand previousPageCommand;
//        private IMvxAsyncCommand nextPageCommand;
//        private MvxAsyncCommand refreshCommand;
//        private MvxAsyncCommand createCommand;
//        private MvxAsyncCommand<TEntity> editCommand;
//        private MvxAsyncCommand<TEntity> deleteCommand;

//        #endregion

//        #region Constructor

//        protected AItemsViewModel(
//            IMvxLogProvider logProvider,
//            IMvxNavigationService navigationService)
//            : base(logProvider, navigationService)
//        { }

//        #endregion

//        #region Properties

//        protected abstract int PageSize { get; }

//        protected virtual string IncludeProperties { get; } = "";

//        public TEntity SelectedItem
//        {
//            get => selectedCustomer;
//            set => SetProperty(ref selectedCustomer, value);
//        }

//        /// <summary> Megjelenitett elemek </summary>
//        public MvxObservableCollection<TEntity> FilteredItems
//        {
//            get => filteredItems;
//            set
//            {
//                SetProperty(ref filteredItems, value);
//                RefreshButtonStates();
//            }
//        }

//        public int CurrentPage
//        {
//            get => currentPage;
//            set => SetProperty(ref currentPage, value);
//        }

//        public string FilterText
//        {
//            get => filterText;
//            set
//            {
//                SetProperty(ref filterText, value);
//                _ = ExecuteSearchCommandAsync(FilterText);
//            }
//        }

//        public IMvxAsyncCommand<string> SearchCommand
//            => searchCommand ?? (searchCommand = new MvxAsyncCommand<string>(ExecuteSearchCommandAsync));

//        public IMvxAsyncCommand PreviousPageCommand
//            => previousPageCommand ?? (previousPageCommand = new MvxAsyncCommand(ExecutePreviousPageCommandAsync, CanExecutePreviousPageCommand));

//        public IMvxAsyncCommand NextPageCommand
//            => nextPageCommand ?? (nextPageCommand = new MvxAsyncCommand(ExecuteNextPageCommandAsync, CanExecuteNextPageCommand));

//        public IMvxAsyncCommand RefreshCommand => refreshCommand ?? (refreshCommand = new MvxAsyncCommand(ExecuteRefreshCommand));

//        public IMvxAsyncCommand CreateCommand => createCommand ?? (createCommand = new MvxAsyncCommand(ExecuteCreateCommand));

//        public IMvxAsyncCommand<TEntity> EditCommand => editCommand ?? (editCommand = new MvxAsyncCommand<TEntity>(ExecuteEditCommand));

//        public IMvxAsyncCommand<TEntity> DeleteCommand => deleteCommand ?? (deleteCommand = new MvxAsyncCommand<TEntity>(ExecuteDeleteCommand));

//        #endregion

//        #region Overrides

//        public override Task Initialize()
//        {
//            // make sure its getting run on backgroundthread
//            ThreadPool.QueueUserWorkItem(_ =>
//            {
//                _ = ExecuteRefreshCommand().ContinueWith(t =>
//                {
//                    FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
//                }).ConfigureAwait(false);
//            });

//            return base.Initialize();
//        }

//        #endregion

//        #region Command execution

//        protected virtual async Task ExecuteRefreshCommand()
//        {
//            if (IsBusy)
//            {
//                return; // a PullToRefresh is ezt hivogatja, ha pl popupot jelenitunk meg....
//            }

//            IsBusy = true;

//            int allItemCount = cachedEntities.Count;

//            await ReloadAsync().ConfigureAwait(false);

//            if (allItemCount != cachedEntities.Count)
//            {
//                CurrentPage = 0;
//            }

//            if (filteredEntities.Any())
//            {
//                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize));
//            }
//            else
//            {
//                FilteredItems = new MvxObservableCollection<TEntity>();
//            }

//            IsBusy = false;
//        }

//        protected virtual async Task ExecuteSearchCommandAsync(string filterTextParam)
//        {
//            if (string.IsNullOrEmpty(filterTextParam))
//            {
//                filteredEntities.Clear();
//                filteredEntities.AddRange(cachedEntities);

//                CurrentPage = 0;
//                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
//                return;
//            }

//            if (filterTextParam?.Length < 3)
//            {
//                return;
//            }

//            searchCancellationTokenSource?.Cancel();
//            searchCancellationTokenSource = new CancellationTokenSource();
//            var searchCancellationToken = searchCancellationTokenSource.Token;

//            try
//            {
//                await InternalFilterAsync(searchCancellationToken, filterTextParam);
//            }
//            catch (Exception exception)
//            {
//                Debug.WriteLine(exception);
//                Crashes.TrackError(exception);
//            }
//            finally
//            {
//                RefreshButtonStates();
//            }
//        }

//        protected virtual async Task ExecuteCreateCommand()
//        {
//            try
//            {
//                IsBusy = true;

//                var param = CreateEditorParameter();
//                var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

//                await ExecuteRefreshCommand();
//            }
//            catch (Exception e)
//            {
//                Crashes.TrackError(e);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        protected virtual async Task ExecuteEditCommand(TEntity entity)
//        {
//            try
//            {
//                IsBusy = true;

//                var param = CreateEditorParameter();
//                param.Data = entity;
//                var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

//                await ExecuteRefreshCommand();
//            }
//            catch (Exception e)
//            {
//                Crashes.TrackError(e);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        protected virtual async Task ExecuteDeleteCommand(TEntity entity)
//        {
//            try
//            {
//                IsBusy = true;

//                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
//                {
//                    await repository.DeleteAsync(entity);
//                }

//                FilteredItems.Remove(entity);
//                filteredEntities.Remove(entity);
//                cachedEntities.Remove(entity);

//                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
//            }
//            catch (Exception e)
//            {
//                Crashes.TrackError(e);
//            }
//            finally
//            {
//                IsBusy = false;
//                RefreshButtonStates();
//            }
//        }

//        protected virtual bool CanExecutePreviousPageCommand()
//            => CurrentPage != 0 && filteredEntities?.Count > 0;

//        protected virtual Task ExecutePreviousPageCommandAsync()
//        {
//            if (!CanExecutePreviousPageCommand())
//            {
//                return Task.CompletedTask;
//            }

//            CurrentPage -= 1;
//            FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize));

//            RefreshButtonStates();
//            return Task.CompletedTask;
//        }

//        /// <summary> Csak akkor van ertelme kovetkezo oldalnak, ha: tobb mint egy oldalny adat van es NEM az utolso oldalon vagyunk </summary>
//        protected virtual bool CanExecuteNextPageCommand()
//            => FilteredItems?.Count == PageSize && filteredEntities.Count > PageSize;

//        protected virtual Task ExecuteNextPageCommandAsync()
//        {
//            if (!CanExecuteNextPageCommand())
//            {
//                return Task.CompletedTask;
//            }

//            CurrentPage += 1;
//            var nextSet = filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize);
//            if (nextSet.Any())
//            {
//                FilteredItems = new MvxObservableCollection<TEntity>(nextSet);
//            }
//            else
//            {
//                CurrentPage -= 1; // cancel
//            }

//            RefreshButtonStates();
//            return Task.CompletedTask;
//        }

//        #endregion

//        protected virtual async Task ReloadAsync()
//        {
//            try
//            {
//                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
//                {
//                    var dbEntities = await repository.GetAsync(includeProperties: IncludeProperties);
//                    cachedEntities.Clear();
//                    cachedEntities.AddRange(dbEntities);

//                    filteredEntities.Clear();
//                    filteredEntities.AddRange(cachedEntities);
//                }
//            }
//            catch (Exception exception)
//            {
//                cachedEntities.Clear();
//                filteredEntities.Clear();

//                Logger?.TraceException(exception.Message, exception);
//            }
//            finally
//            {
//                RefreshButtonStates();
//            }
//        }

//        protected virtual async Task InternalFilterAsync(CancellationToken searchCancellationToken, string filterTextParam)
//        {
//            using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
//            {
//                var entities = await repository.FilterAsync(filterTextParam, cancellationToken: searchCancellationToken);
//                filteredEntities.Clear();
//                filteredEntities.AddRange(entities);

//                CurrentPage = 0;
//                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
//            }
//        }

//        protected virtual void RefreshButtonStates()
//        {
//            PreviousPageCommand.RaiseCanExecuteChanged();
//            NextPageCommand.RaiseCanExecuteChanged();
//        }

//        protected virtual EditorViewModelViewModelParam<TEntity> CreateEditorParameter()
//        {
//            return new EditorViewModelViewModelParam<TEntity>();
//        }
//    }
//}
