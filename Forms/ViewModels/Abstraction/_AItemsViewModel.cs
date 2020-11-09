using Brupper.Data;
using Brupper.Data.Entities;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.ViewModels
{
    public abstract class AItemsViewModel<TEntity, TRepository, TEditViewModel> : AItemsSearchViewModel<TEntity>
        where TEntity : BaseEntity, new()
        where TRepository : class, IRepository<TEntity>, IFilterableRepository<TEntity>
        where TEditViewModel : AEditorViewModel<TEntity, TRepository>
    {
        #region Fields

        private IMvxAsyncCommand previousPageCommand;
        private IMvxAsyncCommand nextPageCommand;
        private MvxAsyncCommand createCommand;
        private MvxAsyncCommand<TEntity> editCommand;
        private MvxAsyncCommand<TEntity> deleteCommand;

        #endregion

        #region Constructor

        protected AItemsViewModel(
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        public IMvxAsyncCommand PreviousPageCommand
            => previousPageCommand ?? (previousPageCommand = new MvxAsyncCommand(ExecutePreviousPageCommandAsync, CanExecutePreviousPageCommand));

        public IMvxAsyncCommand NextPageCommand
            => nextPageCommand ?? (nextPageCommand = new MvxAsyncCommand(ExecuteNextPageCommandAsync, CanExecuteNextPageCommand));

        public IMvxAsyncCommand CreateCommand => createCommand ?? (createCommand = new MvxAsyncCommand(ExecuteCreateCommand));

        public IMvxAsyncCommand<TEntity> EditCommand => editCommand ?? (editCommand = new MvxAsyncCommand<TEntity>(ExecuteEditCommand));

        public IMvxAsyncCommand<TEntity> DeleteCommand => deleteCommand ?? (deleteCommand = new MvxAsyncCommand<TEntity>(ExecuteDeleteCommand));

        #endregion

        #region Command execution

        protected virtual async Task ExecuteCreateCommand()
        {
            try
            {
                IsBusy = true;

                var param = CreateEditorParameter();
                var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

                await ExecuteRefreshCommand();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ExecuteEditCommand(TEntity entity)
        {
            try
            {
                IsBusy = true;

                var param = CreateEditorParameter();
                param.Data = entity;
                var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

                await ExecuteRefreshCommand();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ExecuteDeleteCommand(TEntity entity)
        {
            try
            {
                IsBusy = true;

                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
                {
                    await repository.DeleteAsync(entity);
                }

                FilteredItems.Remove(entity);
                filteredEntities.Remove(entity);
                cachedEntities.Remove(entity);

                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            finally
            {
                IsBusy = false;
                RefreshButtonStates();
            }
        }

        protected virtual bool CanExecutePreviousPageCommand()
            => CurrentPage != 0 && filteredEntities?.Count > 0;

        protected virtual Task ExecutePreviousPageCommandAsync()
        {
            if (!CanExecutePreviousPageCommand())
            {
                return Task.CompletedTask;
            }

            CurrentPage -= 1;
            FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize));

            RefreshButtonStates();
            return Task.CompletedTask;
        }

        /// <summary> Csak akkor van ertelme kovetkezo oldalnak, ha: tobb mint egy oldalny adat van es NEM az utolso oldalon vagyunk </summary>
        protected virtual bool CanExecuteNextPageCommand()
            => FilteredItems?.Count == PageSize && filteredEntities.Count > PageSize;

        protected virtual Task ExecuteNextPageCommandAsync()
        {
            if (!CanExecuteNextPageCommand())
            {
                return Task.CompletedTask;
            }

            CurrentPage += 1;
            var nextSet = filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize);
            if (nextSet.Any())
            {
                FilteredItems = new MvxObservableCollection<TEntity>(nextSet);
            }
            else
            {
                CurrentPage -= 1; // cancel
            }

            RefreshButtonStates();
            return Task.CompletedTask;
        }

        #endregion

        protected override async Task ReloadAsync()
        {
            try
            {
                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
                {
                    var dbEntities = await repository.GetAsync(includeProperties: IncludeProperties);
                    cachedEntities.Clear();
                    cachedEntities.AddRange(dbEntities);

                    filteredEntities.Clear();
                    filteredEntities.AddRange(cachedEntities);
                }
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
        }

        protected override async Task<IEnumerable<TEntity>> InternalFilterAsync(CancellationToken searchCancellationToken, string filterTextParam)
        {
            using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
            {
                var entities = await repository.FilterAsync(filterTextParam, cancellationToken: searchCancellationToken);
                return entities;
            }
        }

        protected override void RefreshButtonStates()
        {
            PreviousPageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
        }

        protected virtual EditorViewModelViewModelParam<TEntity> CreateEditorParameter()
        {
            return new EditorViewModelViewModelParam<TEntity>();
        }
    }
}
