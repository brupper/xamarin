using MvvmCross.Commands;
using MvvmCross.Navigation;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Popups
{
    public class AlertViewModelParameter
    {
        public string Message { get; set; }

        public bool HasException { get; set; }
    }

    public class AlertViewModel : MvxPopupViewModel<AlertViewModelParameter>
    {
        private IMvxAsyncCommand okCommand;
        private AlertViewModelParameter model;

        #region Constructor

        public AlertViewModel(
            IMvxNavigationService navigationService)
            : base(navigationService)
        {
        }

        #endregion

        #region Properties

        public IMvxAsyncCommand OkCommand
        => okCommand ?? (okCommand = new MvxAsyncCommand(ExceuteOkCommand));

        public IMvxAsyncCommand CancelCommand
            => BackCommand;

        public AlertViewModelParameter Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        #endregion

        #region Overrides

        public override void Prepare(AlertViewModelParameter parameter)
        {
            Model = parameter;
        }

        #endregion

        private async Task ExceuteOkCommand(CancellationToken arg)
        {
            // TODO: some logic
            await ExecuteBackCommandAsync();
        }
    }
}
