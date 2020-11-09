using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using XfView = Xamarin.Forms.View;

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

        /// <summary> Js to get the height of HTML content. The .toString() is extremely important! </summary>
        public const string JavaSciptGetHeight = "document.body.getBoundingClientRect().height.toString() || document.body.clientHeight.toString()";

        /// <summary> We have to normalize height of the WebView and this script helps to get the HTML size. (Window height) </summary>
        public const string JavaSciptGetWindowHeight = "document.documentElement.clientHeight.toString() || window.innerHeight.toString()";

        /// <summary> Js to get the height of HTML content. The .toString() is extremely important! </summary>
        public const string JavaSciptGetWidth = "document.body.getBoundingClientRect().width.toString() || document.body.clientWidth.toString()";

        /// <summary> We have to normalize height of the WebView and this script helps to get the HTML size. (Window height) </summary>
        public const string JavaSciptGetWindowWidth = "document.documentElement.clientWidth.toString() || window.innerWidth.toString()";

        /// <summary> Offset for webview's dynamic height. Vertical scrollbar must not show. </summary>
        public const int HtmlContentHeightPaddingOffset = 18;

        /// <summary> We have to normalize height of the WebView to remove ugly scrollbars. </summary>
        public static async Task<double> NormalizeHeightOfWebView(this WebView webView)
        {
            if (webView.Source == null)
            {
                return 0;
            }

            var heightString = await webView.EvaluateJavaScriptAsync(ViewExtensions.JavaSciptGetHeight) ?? string.Empty;
            var windowHeightString = await webView.EvaluateJavaScriptAsync(ViewExtensions.JavaSciptGetWindowHeight) ?? string.Empty;

            double height, windowHeight;
            //TODO: use invariant culture.
            if ((double.TryParse(heightString, out height) || double.TryParse(heightString.Replace('.', ','), out height))
                && (double.TryParse(windowHeightString, out windowHeight) || double.TryParse(windowHeightString.Replace('.', ','), out windowHeight)))
            {
                if (webView.HeightRequest != height
                    && Math.Abs(webView.HeightRequest - height) >= ViewExtensions.HtmlContentHeightPaddingOffset)
                {
                    height += ViewExtensions.HtmlContentHeightPaddingOffset;
                    webView.HeightRequest = height;
                    return height;
                }

                //TODO: normalize somehow: webView.HeightRequest = webView.Height > 0 && windowHeight > 0 ? height * (webView.Height / windowHeight) : height;
            }
            return webView.Height;
        }

        /// <summary> . </summary>
        public static XfView CreateContentForItem(this object itemTemplate, object item, ICommand itemTappedCommand, BindableObject parent = null)
        {
            XfView itemView = null;

            if (itemTemplate is DataTemplateSelector selector)
            {
                var view = selector.SelectTemplate(item, parent).CreateContent();
                if (view is ViewCell cell)
                {
                    itemView = cell.View;
                }
                else
                {
                    itemView = view as View;
                }
            }
            else if (itemTemplate is DataTemplate template)
            {
                itemView = template.CreateContent() as XfView;
            }


            if (itemView == null)
            {
                throw new ArgumentException("Can not create View.");
            }

            if (item != null)
            {
                itemView.BindingContext = item;
            }

            if (itemTappedCommand != null)
            {
                var tapRecognizer = new TapGestureRecognizer
                {
                    Command = itemTappedCommand,
                    CommandParameter = item
                };
                itemView.GestureRecognizers.Add(tapRecognizer);
            }
            return itemView;
        }
    }
}
