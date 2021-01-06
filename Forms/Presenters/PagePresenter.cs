using Brupper.Forms.Attributes;
using MvvmCross.Forms.Presenters;
using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Presenters;
using MvvmCross.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;

namespace Brupper.Forms.Presenters
{
    public class PagePresenter : MvxFormsPagePresenter
    {
        public PagePresenter(IMvxFormsViewPresenter platformPresenter)
            : base(platformPresenter)
        {
        }

        public override void RegisterAttributeTypes()
        {
            base.RegisterAttributeTypes();

            AttributeTypesToActionsDictionary.Add(
                typeof(PopupPresentationAttribute),
                new MvxPresentationAttributeAction
                {
                    ShowAction = (view, attribute, request) => ShowPopupPage(view, (PopupPresentationAttribute)attribute, request),
                    CloseAction = (viewModel, attribute) => ClosePopupPage(viewModel, (PopupPresentationAttribute)attribute)
                });
        }

        private async Task<bool> ShowPopupPage(Type view, PopupPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var mainPage = FormsApplication.MainPage;

            var page = CreatePage(view, request, attribute) as PopupPage;
            await mainPage.Navigation.PushPopupAsync(page, attribute.Animated);

            return true;
        }

        private async Task<bool> ClosePopupPage(IMvxViewModel viewModel, PopupPresentationAttribute attribute)
        {
            var mainPage = FormsApplication.MainPage;

            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {

                await mainPage.Navigation.PopPopupAsync(attribute.Animated);
            }

            return true;
        }

        public override Task<bool> ShowContentPage(Type view, MvxContentPagePresentationAttribute attribute, MvxViewModelRequest request)
        {
            var presentationValues = request.PresentationValues;
            if (presentationValues != null && presentationValues.TryGetValue(nameof(attribute.NoHistory), out string presentationValue))
            {
                if (bool.TryParse(presentationValue, out bool noHistory))
                {
                    attribute.NoHistory = noHistory;
                }
            }

            return base.ShowContentPage(view, attribute, request);
        }

        public override Task<bool> ShowCarouselPage(Type view, MvvmCross.Forms.Presenters.Attributes.MvxCarouselPagePresentationAttribute attribute, MvxViewModelRequest request)
        {
            attribute.Position = MvvmCross.Forms.Presenters.Attributes.CarouselPosition.Root;
            return base.ShowCarouselPage(view, attribute, request);
        }
    }
}
