using CoreGraphics;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Brupper.Forms.Platforms.iOS.Services
{
    // https://forums.xamarin.com/discussion/174630/how-to-convert-webview-source-html-string-to-image-format
    internal class PngExportWebViewCallBack : UIWebViewDelegate
    {
        private readonly TaskCompletionSource<string> source;
        private readonly string filePath;

        public PngExportWebViewCallBack(TaskCompletionSource<string> source, string path)
        {
            this.source = source;
            filePath = path;
        }

        public override void LoadingFinished(UIWebView webView)
        {
            try
            {
                var htmlSize = webView.GetInnerSize();

                double height = htmlSize.height, width = htmlSize.width;
                int header = 10, sidespace = 10;

                var pageMargins = new UIEdgeInsets(header, sidespace, header, sidespace);
                webView.ViewPrintFormatter.ContentInsets = pageMargins;

                var renderer = new UIPrintPageRenderer();
                renderer.AddPrintFormatter(webView.ViewPrintFormatter, 0);

                var pageSize = new CGSize(width, height);
                var printableRect = new CGRect(sidespace, header, pageSize.Width - (sidespace * 2), pageSize.Height - (header * 2));
                var paperRect = new CGRect(0, 0, pageSize.Width, pageSize.Height);
                renderer.SetValueForKey(NSValue.FromObject(paperRect), (NSString)"paperRect");
                renderer.SetValueForKey(NSValue.FromObject(printableRect), (NSString)"printableRect");

                UIGraphics.BeginImageContext(new CGSize(paperRect.Width, paperRect.Height));
                renderer.PrepareForDrawingPages(new NSRange(0, renderer.NumberOfPages));
                for (int i = 0; i < renderer.NumberOfPages; i++)
                {
                    renderer.DrawPage(i, paperRect);
                }

                var fullImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                fullImage.AsPNG().Save(filePath, false);

                source.SetResult(filePath);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
                source.SetException(exception);
            }
        }
    }
}