﻿using Brupper.Data;
using Brupper.Data.Entities;
using MvvmCross;
using MvvmCross.Commands;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Abstraction
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
            ILoggerFactory logProvider,
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

                await TryExecuteCreateCommand();
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                await ShowAlertWithKey(null);
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
                await TryExecuteEditCommand(entity);
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                await ShowAlertWithKey(null);
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
                await TryExecuteDeleteCommand(entity);
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                await ShowAlertWithKey("general_error_delete");
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
            try
            {
                IsBusy = true;

                if (!CanExecutePreviousPageCommand())
                {
                    return Task.CompletedTask;
                }

                CurrentPage -= 1;
                FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Skip(PageSize * CurrentPage).Take(PageSize));

                RefreshButtonStates();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                return ShowAlertWithKey(null);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary> Csak akkor van ertelme kovetkezo oldalnak, ha: tobb mint egy oldalny adat van es NEM az utolso oldalon vagyunk </summary>
        protected virtual bool CanExecuteNextPageCommand()
            => FilteredItems?.Count == PageSize && filteredEntities.Count > PageSize;

        protected virtual Task ExecuteNextPageCommandAsync()
        {
            try
            {
                IsBusy = true;

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
            catch (Exception e)
            {
                Logger.TrackError(e);
                return ShowAlertWithKey(null);
            }
            finally
            {
                IsBusy = false;
            }
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

                Logger?.TrackError(exception);
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
                return entities.ToList();
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

        protected virtual async Task TryExecuteCreateCommand()
        {
            var param = CreateEditorParameter();
            var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

            using (stateHolder)
                await ExecuteRefreshCommand().ConfigureAwait(false);
        }

        protected virtual async Task TryExecuteEditCommand(TEntity entity)
        {
            var param = CreateEditorParameter();
            param.Data = entity;
            var stateHolder = await NavigationService.Navigate<TEditViewModel, EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>(param);

            using (stateHolder)
                await ExecuteRefreshCommand().ConfigureAwait(false);
        }

        protected virtual async Task TryExecuteDeleteCommand(TEntity entity)
        {
            using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
            {
                await repository.DeleteAsync(entity).ConfigureAwait(false);
            }

            FilteredItems.Remove(entity);
            filteredEntities.Remove(entity);
            cachedEntities.Remove(entity);

            FilteredItems = new MvxObservableCollection<TEntity>(filteredEntities.Take(PageSize));
        }
    }
}
