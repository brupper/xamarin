using Android.Graphics;
using Android.Graphics.Pdf;
using Android.Webkit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Brupper.Forms.Platforms.Android.PlatformServices
{
    // https://stackoverflow.com/questions/37340813/xamarin-free-html-or-doc-to-pdf-conversion
    public class PdfExportWebViewCallBack : WebViewClient
    {
        private TaskCompletionSource<string> source;
        private string fileNameWithPath = null;
        private bool checkOnPageStartedCalled;

        public PdfExportWebViewCallBack(TaskCompletionSource<string> source, string path)
        {
            this.source = source;
            fileNameWithPath = path;
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
                webview.Layout(0, 0, htmlSize.width, htmlSize.height);// magic happens

                var document = new PdfDocument();
                var page = document.StartPage(new PdfDocument.PageInfo.Builder(htmlSize.width, htmlSize.height, 1).Create());

                webview.Draw(page.Canvas);
                document.FinishPage(page);

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
