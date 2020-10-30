using Brupper.Forms.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.ViewModels.Popups
{
    public class QuestionViewModelParam
    {
        public string LocalizedQuestion { get; set; }

        public DialogButtonModel OkButton { get; set; }
            = new DialogButtonModel { TranslateKey = "general_ok" };

        public DialogButtonModel CancelButton { get; set; }
            = new DialogButtonModel { TranslateKey = "general_cancel" };

        public IEnumerable<DialogButtonModel> AdditionalButtons { get; set; }
            = Enumerable.Empty<DialogButtonModel>();

    }

    public class QuestionViewModel : MvxPopupViewModel<QuestionViewModelParam, bool>
    {
        #region Fields

        private ICommand internalOkCommand;
        private ICommand internalCancelCommand;

        private string _question;
        private IMvxAsyncCommand _acceptCommand;
        private MvxObservableCollection<DialogButtonModel> buttons = new MvxObservableCollection<DialogButtonModel>();

        #endregion

        #region Constructors

        public QuestionViewModel(IMvxNavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Properties

        public MvxObservableCollection<DialogButtonModel> Buttons
        {
            get => buttons;
            set => SetProperty(ref buttons, value);
        }

        public string Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        public IMvxAsyncCommand AcceptCommand
            => _acceptCommand ?? (_acceptCommand = new MvxAsyncCommand(ExecuteAcceptCommand));

        #endregion

        public override void Prepare(QuestionViewModelParam parameter)
        {
            Question = parameter.LocalizedQuestion;

            if (parameter.OkButton != null)
            {
                internalOkCommand = parameter.OkButton.Command;
                parameter.OkButton.Command = AcceptCommand;
                Buttons.Add(parameter.OkButton);
            }

            if (parameter.CancelButton != null)
            {
                internalCancelCommand = parameter.CancelButton.Command;
                parameter.CancelButton.Command = BackPressedCommand;
                Buttons.Add(parameter.CancelButton);
            }

            foreach (var item in parameter.AdditionalButtons)
            {
                Buttons.Add(item);
            }
        }

        protected override Task ExecuteBackCommandAsync()
        {
            internalCancelCommand?.Execute(null);
            return navigationService.Close(this, false);
        }

        private Task ExecuteAcceptCommand()
        {
            internalOkCommand?.Execute(null);
            return navigationService.Close(this, true);
        }
    }
}
