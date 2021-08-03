using Android.Graphics;
using Android.Print;
using Android.Webkit;
using Brupper.Forms.Models.Rendering;
using System.Threading.Tasks;

namespace Brupper.Forms.Platforms.Android.Services
{
    public class PdfVectorExportWebViewCallBack : WebViewClient
    {
        private TaskCompletionSource<string> source;
        private string fileNameWithPath;
        private bool checkOnPageStartedCalled;
        private int numberOfPages;
        private PaperSize paperSize;

        public PdfVectorExportWebViewCallBack(TaskCompletionSource<string> source, string path, int numberOfPages, PaperSize pageSize)
        {
            this.source = source;
            fileNameWithPath = path;
            this.numberOfPages = numberOfPages;
            this.paperSize = pageSize;
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);
            checkOnPageStartedCalled = true;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            if (!checkOnPageStartedCalled)
            {
                OnPageFinished(view, url);
                return;
            }

            view.SetLayerType(global::Android.Views.LayerType.Software, null);

            var printAttributes = new PrintAttributes.Builder()
                            .SetMediaSize(PrintAttributes.MediaSize.IsoA4)
                            .SetResolution(new PrintAttributes.Resolution(id: "pdf", label: "pdf", 300, 300))
                            .SetMinMargins(PrintAttributes.Margins.NoMargins)
                            .SetDuplexMode(DuplexMode.None)
                            .Build();


            var context = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;

            var printAdapter = view.CreatePrintDocumentAdapter("pdf_export");

            var callback = new LayoutResultCallback(printAdapter, source, fileNameWithPath);
            printAdapter.OnLayout(oldAttributes: null, printAttributes, cancellationSignal: null, callback, extras: null);

            // source.SetResult(fileNameWithPath); => will be set by WriteResultCallback inside LayoutResultCallback ... 
        }
    }
}