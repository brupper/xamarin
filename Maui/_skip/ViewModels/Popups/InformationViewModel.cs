using Microsoft.Maui.Controls;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Brupper.ViewModels.Popups
{
    public class InformationViewModelParam
    {
        public bool IsError { get; set; }
        public string Header { get; set; }
        public IEnumerable<string> Lines { get; set; } = new string[0];
        public string Html { get; set; }
    }

    public class InformationViewModel : MvxPopupViewModel<InformationViewModelParam>
    {
        #region Fields

        private InformationViewModelParam parameter = new InformationViewModelParam();
        private bool isError;
        private bool isHtml;
        private HtmlWebViewSource htmlSource;
        private string header;
        private MvxObservableCollection<string> lines = new MvxObservableCollection<string>();
        private bool hasLines;

        #endregion

        #region Contructors
        public InformationViewModel(IMvxNavigationService navigationService)
            : base(navigationService)
        { }

        #endregion

        #region Properties

        public bool IsError
        {
            get => isError;
            set => SetProperty(ref isError, value);
        }

        public bool IsHtml
        {
            get => isHtml;
            set => SetProperty(ref isHtml, value);
        }

        public HtmlWebViewSource HtmlSource

        {
            get => htmlSource;
            set => SetProperty(ref htmlSource, value);
        }

        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        public MvxObservableCollection<string> Lines
        {
            get => lines;
            set => SetProperty(ref lines, value);
        }

        public bool HasLines
        {
            get => hasLines;
            set => SetProperty(ref hasLines, value);
        }

        #region Execute commands

        #endregion

        #endregion

        public override void Prepare(InformationViewModelParam parameter)
        {
            this.parameter = parameter ?? new InformationViewModelParam();
            IsError = parameter.IsError;
            Header = parameter.Header;
            Lines = new MvxObservableCollection<string>(parameter.Lines);
            HasLines = Lines.Any();

            if (!string.IsNullOrEmpty(parameter.Html))
            {
                HtmlSource = new HtmlWebViewSource { Html = parameter.Html };
                IsHtml = true;
            }
        }
    }
}
