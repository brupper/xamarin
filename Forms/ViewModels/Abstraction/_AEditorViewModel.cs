using Brupper.Data;
using Brupper.Data.Entities;
using Brupper.ViewModels.Popups;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brupper.ViewModels
{
    public class EditorViewModelViewModelParam<TEntity>
    {
        public TEntity Data { get; set; }

        public SimpleStateHolder StateHolder { get; set; }
            = new SimpleStateHolder(true);
    }

    public abstract class AEditorViewModel<TEntity, TRepository> : ViewModelBase<EditorViewModelViewModelParam<TEntity>, SimpleStateHolder>
        where TRepository : class, IRepository<TEntity>
        where TEntity : BaseEntity, new()
    {
        #region Fields

        protected EditorViewModelViewModelParam<TEntity> param;
        protected bool createNew;

        private MvxAsyncCommand saveCommand;
        private MvxAsyncCommand deleteCommand;
        private TEntity entity;
        private string title;
        private string saveTitle;

        #endregion

        #region Constructor

        public AEditorViewModel(
            IMvxLogProvider logProvider
            , IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        { }

        #endregion

        #region Properties

        public IMvxAsyncCommand SaveCommand => saveCommand ?? (saveCommand = new MvxAsyncCommand(ExecuteSaveCommand));

        public IMvxAsyncCommand DeleteCommand => deleteCommand ?? (deleteCommand = new MvxAsyncCommand(ExecuteDeleteCommand));

        public TEntity Entity
        {
            get => entity;
            set => SetProperty(ref entity, value);
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string SaveTitle
        {
            get => saveTitle;
            set => SetProperty(ref saveTitle, value);
        }

        public bool IsEditing => !createNew;

        #endregion

        #region Execute commands

        protected override Task ExecuteBackPressedCommandAsync()
            => NavigationService.Close(this, param.StateHolder);

        protected virtual async Task ExecuteSaveCommand()
        {
            try
            {
                IsBusy = true;

                if (!(await ValidateEntityAsync()))
                {
                    await ShowValidationErrorsAsync();
                    return;
                }

                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
                {
                    if (createNew)
                    {
                        await repository.InsertAsync(Entity);
                    }
                    else
                    {
                        await repository.UpdateAsync(Entity);
                    }

                    await NavigationService.Close(this, param.StateHolder);
                }
            }
            catch (Exception e)
            {
                Microsoft.AppCenter.Crashes.Crashes.TrackError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ExecuteDeleteCommand()
        {
            try
            {
                IsBusy = true;

                using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
                    await repository.DeleteAsync(Entity);

                await NavigationService.Close(this, param.StateHolder);
            }
            catch (Exception e)
            {
                Microsoft.AppCenter.Crashes.Crashes.TrackError(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Overrides

        public override void Prepare(EditorViewModelViewModelParam<TEntity> parameter)
        {
            param = parameter = parameter ?? new EditorViewModelViewModelParam<TEntity>();
            createNew = parameter.Data == null;
            Entity = parameter.Data;
        }

        public override async Task Initialize()
        {
            if (Entity == null)
            {
                Entity = new TEntity();
            }

            await base.Initialize();
        }

        #endregion

        protected virtual Task<bool> ValidateEntityAsync()
        {
            //TODO: Entity.IsValid
            return Task.FromResult(true);
        }

        protected virtual Task ShowValidationErrorsAsync()
            => NavigationService.Navigate<InformationViewModel, InformationViewModelParam>(new InformationViewModelParam
            {
                Header = TextProvider.GetText(null, null, "error_header"),
                Lines = new List<string> { TextProvider.GetText(null, null, "entity_validation_error") },
            });
    }
}
