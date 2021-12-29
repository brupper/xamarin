using Brupper.Data;
using Brupper.Data.Entities;
using MvvmCross;
using MvvmCross.Commands;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;
using System;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Abstraction
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
        private bool createNew;

        private MvxAsyncCommand saveCommand;
        private MvxAsyncCommand deleteCommand;
        private TEntity entity;
        private string title;
        private string saveTitle;

        #endregion

        #region Constructor

        public AEditorViewModel(
            ILoggerFactory logProvider
            , IMvxNavigationService navigationService)
            : base(logProvider, navigationService)
        {
        }

        #endregion

        #region Properties

        public IMvxAsyncCommand SaveCommand => saveCommand ?? (saveCommand = new MvxAsyncCommand(ExecuteSaveCommand));

        public IMvxAsyncCommand DeleteCommand => deleteCommand ?? (deleteCommand = new MvxAsyncCommand(ExecuteDeleteCommand));

        public virtual TEntity Entity
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

        public virtual bool IsEditing => !createNew;

        #endregion

        #region Execute commands

        protected override Task ExecuteBackPressedCommandAsync()
        {
            return this.Close(this, param.StateHolder);
        }

        protected virtual async Task ExecuteSaveCommand()
        {
            try
            {
                IsBusy = true;
                await TryExecuteSaveCommandAsync();
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                await ShowAlertWithKey("general_error_save");
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
                    await repository.DeleteAsync(Entity).ConfigureAwait(false);

                await this.Close(this, param.StateHolder);
            }
            catch (Exception e)
            {
                Logger.TrackError(e);
                await ShowAlertWithKey("general_error_delete");
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

        protected abstract Task<bool> ValidateShowValidationErrorsAsync();

        protected virtual async Task TryExecuteSaveCommandAsync()
        {
            if (!(await ValidateShowValidationErrorsAsync()))
            {
                return;
            }

            using (var repository = Mvx.IoCProvider.Resolve<TRepository>())
            {
                if (IsEditing)
                {
                    await repository.UpdateAsync(Entity).ConfigureAwait(false);
                }
                else
                {
                    await repository.InsertAsync(Entity).ConfigureAwait(false);
                }

                await this.Close(this, param.StateHolder);
            }
        }
    }
}
