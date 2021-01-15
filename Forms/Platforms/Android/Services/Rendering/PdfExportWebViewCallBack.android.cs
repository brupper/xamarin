using Android.Graphics;
using Android.Graphics.Pdf;
using Android.Webkit;
using Android.Widget;
using Brupper.Forms.Models.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Brupper.Forms.Platforms.Android.Services
{
    // https://stackoverflow.com/questions/37340813/xamarin-free-html-or-doc-to-pdf-conversion
    public class PdfExportWebViewCallBack : WebViewClient
    {
        private TaskCompletionSource<string> source;
        private string fileNameWithPath;
        private bool checkOnPageStartedCalled;
        private int numberOfPages;
        private PaperSize paperSize;

        public PdfExportWebViewCallBack(TaskCompletionSource<string> source, string path, int numberOfPages, PaperSize pageSize)
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

        public override async void OnPageFinished(WebView webview, string url)
        {
            try
            {
                if (!checkOnPageStartedCalled)
                {
                    OnPageFinished(webview, url);
                    return;
                }

                base.OnPageFinished(webview, url);
                await Task.Delay(100);

                var htmlSize = await webview.GetInnerSize();
                var document = new PdfDocument();

                if (numberOfPages == 1)
                {
                    webview.Layout(0, 0, htmlSize.width, htmlSize.height);// magic happens


                    var page = document.StartPage(new PdfDocument.PageInfo.Builder(htmlSize.width, htmlSize.height, 1).Create());

                    webview.Draw(page.Canvas);
                    document.FinishPage(page);
                }
                else
                {
                    var pageHeight = htmlSize.width * paperSize.PageRatio;

                    if (pageHeight <= double.Epsilon) pageHeight = htmlSize.height; // fallback

                    // Webview is added into a frame layout then the whole webview is repositioned for each page
                    var frameLayout = new FrameLayout(webview.Context);
                    frameLayout.LayoutParameters = new FrameLayout.LayoutParams(htmlSize.width, htmlSize.height);
                    webview.LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, FrameLayout.LayoutParams.MatchParent);
                    frameLayout.AddView(webview);

                    // Rendering
                    for (int i = 0; i < numberOfPages; i++)
                    {
                        var page = document.StartPage(new PdfDocument.PageInfo.Builder(htmlSize.width, (int)pageHeight, i + 1).Create());
                        webview.SetY((int)(-pageHeight * i));
                        frameLayout.Draw(page.Canvas);
                        document.FinishPage(page);
                    }
                }

                using (var fs = File.Create(fileNameWithPath))
                {
                    document.WriteTo(fs);
                }

                source.SetResult(fileNameWithPath);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
                source.SetException(exception);
            }
        }

        //public override void OnPageFinished(WebView view, string url)
        //{
        //    var printAdapter = view.CreatePrintDocumentAdapter(System.IO.Path.GetFileNameWithoutExtension(fileNameWithPath));

        //    var printAttributes = new PrintAttributes.Builder()
        //                    .SetMediaSize(PrintAttributes.MediaSize.IsoA4)
        //                    .SetResolution(new PrintAttributes.Resolution("pdf", "pdf", 600, 600))
        //                    .SetMinMargins(PrintAttributes.Margins.NoMargins)
        //                    .Build();


        //    var callback = new Android.Print.PdfPrintCallback(printAdapter, fileNameWithPath);
        //    //callback.OnLayoutFinished(null, false);

        //    printAdapter.OnLayout(null
        //        , printAttributes
        //        , new CancellationSignal()
        //        , callback
        //        , new Bundle());
        //    source.SetResult(fileNameWithPath);
        //}
    }
}

//namespace Android.Print
//{
//    // javac.exe error JAVAC0000:  error: LayoutResultCallback() is not public in LayoutResultCallback; cannot be accessed from outside package
//    // javac.exe error JAVAC0000: public class PdfPrintCallback
//    [Register("android/print/PrintDocumentAdapter$LayoutResultCallback", DoNotGenerateAcw = true)]
//    public class PdfPrintCallback : PrintDocumentAdapter.LayoutResultCallback
//    {
//        PrintDocumentAdapter printAdapter;
//        string fileNameWithPath;

//        #region Constructor

//        // JNIEnv.Handle, JniHandleOwnership.DoNotRegister
//        // IntPtr.Zero, JniHandleOwnership.DoNotTransfer
//        public PdfPrintCallback(PrintDocumentAdapter printAdapter, string fileNameWithPath)
//            : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
//        {
//            this.printAdapter = printAdapter;
//            this.fileNameWithPath = fileNameWithPath;
//        }

//        // public PdfPrintCallback(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

//        #endregion

//        public override void OnLayoutFinished(PrintDocumentInfo info, bool changed)
//        {
//            //base.OnLayoutFinished(info, changed);
//            var fd = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(fileNameWithPath), ParcelFileMode.Create);
//            printAdapter.OnWrite(
//                new[] { PageRange.AllPages }
//                , fd
//                , new CancellationSignal()
//                , new MyWriteResultCallback());
//        }
//    }

//    [Register("android/print/MyWriteResultCallback")]
//    public class MyWriteResultCallback : PrintDocumentAdapter.WriteResultCallback
//    {
//        public MyWriteResultCallback()
//            : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
//        { }
//    }
//}
