using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Brupper.Forms
{
    public static class ViewExtensions
    {
        public static Thickness PagePadding = new Thickness(0);

        public static Thickness PagePaddingForCarouselPages = new Thickness(0);

        public static void FixPageOniOS(this Xamarin.Forms.Page page)
        {
            // Get the safe area here, then we just use the Top as the Scroll View's margin
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                var inset = page.On<iOS>().SafeAreaInsets();
                page.Padding = new Thickness(0, (-1 * (inset.Top)), 0, inset.Bottom);

                if (inset.Top != 0 || inset.Bottom != 0)
                {
                    PagePadding = page.Padding;

                    var t = new Thickness(PagePadding.Left, PagePadding.Top, PagePadding.Right, PagePadding.Bottom);
                    t.Top = Math.Abs(t.Top);
                    t.Top = 0d;
                    PagePaddingForCarouselPages = t;
                }

                //if (page is ContentPage cp && cp.Content is Xamarin.Forms.ScrollView scrollView)
                //{
                //    scrollView.Margin = new Thickness(0, 0, 0, inset.Bottom < 24 ? 24 : inset.Bottom);
                //}
            }
        }

        public static void CloseMenu(this Xamarin.Forms.Element view)
        {
            var p = view.Parent;

            int i = 0;// avaoid infinite loop
            while (!(p is MvvmCross.Forms.Views.MvxMasterDetailPage) && i < 50)
            {
                p = p?.Parent;
                i++;
            }

            if (p is MvvmCross.Forms.Views.MvxMasterDetailPage md)
            {
                md.MasterBehavior = MasterBehavior.Popover;
                md.IsPresented = !md.IsPresented;
            }
        }
    }
}
