using Brupper.Forms.UiModels;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.ViewModels.Popups
{
    public class QuestionViewModelParam
    {
        public string LocalizedQuestion { get; set; }

        public DialogButtonUiModel OkButton { get; set; }
            = new DialogButtonUiModel { TranslateKey = "general_ok", Result = true, ShouldClosePopup = true };

        public DialogButtonUiModel CancelButton { get; set; }
            = new DialogButtonUiModel { TranslateKey = "general_cancel", Result = false, ShouldClosePopup = true };

        public IEnumerable<DialogButtonUiModel> AdditionalButtons { get; set; }
            = Enumerable.Empty<DialogButtonUiModel>();
    }

    public class QuestionViewModel : MvxPopupViewModel<QuestionViewModelParam, DialogButtonUiModel>
    {
        #region Fields

        private QuestionViewModelParam parameter;

        private string question;
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

        public IMvxAsyncCommand<DialogButtonUiModel> ButtonCommand
            => buttonCommand ?? (buttonCommand = new MvxAsyncCommand<DialogButtonUiModel>(ExecuteButtonCommandAsync));

        #endregion

        public override void Prepare(QuestionViewModelParam parameter)
        {
            this.parameter = parameter;
            Question = parameter.LocalizedQuestion;

            if (parameter.OkButton != null)
            {
                Buttons.Add(parameter.OkButton);
            }

            if (parameter.CancelButton != null)
            {
                Buttons.Add(parameter.CancelButton);
            }

            foreach (var item in parameter.AdditionalButtons)
            {
                Buttons.Add(item);
            }
        }

        protected override Task ExecuteBackCommandAsync()
        {
            parameter.CancelButton?.Command?.Execute(null);
            return navigationService.Close(this, parameter.CancelButton);
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
                return navigationService.Close(this, button);
            }

            return Task.CompletedTask;
        }
    }
}
