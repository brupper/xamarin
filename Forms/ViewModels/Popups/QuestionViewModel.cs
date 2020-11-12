using Brupper.Forms.UiModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Brupper.ViewModels.Popups
{
    public class QuestionViewModelParam
    {
        public string LocalizedQuestion { get; set; }

        public DialogButtonUiModel OkButton { get; set; }
            = new DialogButtonUiModel { TranslateKey = "general_ok" };

        public DialogButtonUiModel CancelButton { get; set; }
            = new DialogButtonUiModel { TranslateKey = "general_cancel" };

        public IEnumerable<DialogButtonUiModel> AdditionalButtons { get; set; }
            = Enumerable.Empty<DialogButtonUiModel>();
    }

    public class QuestionViewModelResult
    {
        public DialogButtonUiModel SelectedButton { get; set; }

        public bool Result { get; set; }
    }

    public class QuestionViewModel : MvxPopupViewModel<QuestionViewModelParam, QuestionViewModelResult>
    {
        #region Fields

        private QuestionViewModelParam parameter;
        private ICommand internalOkCommand;
        private ICommand internalCancelCommand;

        private string question;
        private IMvxAsyncCommand acceptCommand;
        private IMvxAsyncCommand<DialogButtonUiModel> buttonCommand;
        private MvxObservableCollection<DialogButtonUiModel> buttons = new MvxObservableCollection<DialogButtonUiModel>();

        #endregion

        #region Constructors

        public QuestionViewModel(IMvxNavigationService navigationService) : base(navigationService) { }

        #endregion

        #region Properties

        public MvxObservableCollection<DialogButtonUiModel> Buttons
        {
            get => buttons;
            set => SetProperty(ref buttons, value);
        }

        public string Question
        {
            get => question;
            set => SetProperty(ref question, value);
        }

        public IMvxAsyncCommand AcceptCommand
            => acceptCommand ?? (acceptCommand = new MvxAsyncCommand(ExecuteAcceptCommand));

        public IMvxAsyncCommand<DialogButtonUiModel> ButtonCommand
            => buttonCommand ?? (buttonCommand = new MvxAsyncCommand<DialogButtonUiModel>(ExecuteButtonCommandAsync));

        #endregion

        public override void Prepare(QuestionViewModelParam parameter)
        {
            this.parameter = parameter;
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
            return navigationService.Close(this, new QuestionViewModelResult { SelectedButton = parameter.CancelButton, Result = false });
        }

        private Task ExecuteAcceptCommand()
        {
            internalOkCommand?.Execute(null);
            return navigationService.Close(this, new QuestionViewModelResult { SelectedButton = parameter.OkButton, Result = true });
        }

        private Task ExecuteButtonCommandAsync(DialogButtonUiModel button)
        {
            if (button == null)
            {
                return Task.CompletedTask;
            }

            button.Command?.Execute(button);
            if (button.ShouldClosePopup)
            {
                return navigationService.Close(this, new QuestionViewModelResult { SelectedButton = button, Result = true });
            }

            return Task.CompletedTask;
        }
    }
}
