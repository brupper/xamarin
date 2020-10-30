using CoreGraphics;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Brupper.Forms.Platforms.iOS.Services
{
    // https://stackoverflow.com/questions/37340813/xamarin-free-html-or-doc-to-pdf-conversion
    internal class PdfExportWebViewCallBack : UIWebViewDelegate
    {
        private readonly TaskCompletionSource<string> source;
        private readonly string filePath;

        public PdfExportWebViewCallBack(TaskCompletionSource<string> source, string path)
        {
            this.source = source;
            filePath = path;
        }

        public override void LoadingFinished(UIWebView webView)
        {
            try
            {
                double height, width;
                int header = 10, sidespace = 10;

                //https://www.graphic-design-employment.com/a4-paper-dimensions.html
                width = 595.2;
                height = 841.8;

                var pageMargins = new UIEdgeInsets(header, sidespace, header, sidespace);
                webView.ViewPrintFormatter.ContentInsets = pageMargins;

                var renderer = new UIPrintPageRenderer();
                renderer.AddPrintFormatter(webView.ViewPrintFormatter, 0);

                var pageSize = new CGSize(width, height);
                var printableRect = new CGRect(sidespace, header, pageSize.Width - (sidespace * 2), pageSize.Height - (header * 2));
                var paperRect = new CGRect(0, 0, pageSize.Width, pageSize.Height);
                renderer.SetValueForKey(NSValue.FromObject(paperRect), (NSString)"paperRect");
                renderer.SetValueForKey(NSValue.FromObject(printableRect), (NSString)"printableRect");
                var file = PrintToPDFWithRenderer(renderer, paperRect);
                File.WriteAllBytes(filePath, file.ToArray());

                source.SetResult(filePath);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
                source.SetException(exception);
            }
        }

        private NSData PrintToPDFWithRenderer(UIPrintPageRenderer renderer, CGRect paperRect)
        {
            var pdfData = new NSMutableData();
            UIGraphics.BeginPDFContext(pdfData, paperRect, null);

            renderer.PrepareForDrawingPages(new NSRange(0, renderer.NumberOfPages));

            var bounds = UIGraphics.PDFContextBounds;
            for (int i = 0; i < renderer.NumberOfPages; i++)
            {
                UIGraphics.BeginPDFPage();
                renderer.DrawPage(i, paperRect);
            }
            UIGraphics.EndPDFContent();

            return pdfData;
        }
    }
}