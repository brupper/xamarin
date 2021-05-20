using Android.Webkit;
using Brupper.Forms.Models.Rendering;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Platforms.Android.Services
{
    /// <inheritdoc />
    public class OutputRendererServices : Forms.Services.Concretes.AOutputRendererServices
    {
        /// <inheritdoc />
        public OutputRendererServices(IFileSystem fileSystem)
            : base(fileSystem) { }

        //public override async Task OpenPdfAsync(string filePath)
        //{
        //    var bytes = File.ReadAllBytes(filePath);

        //    //Copy the private file's data to the EXTERNAL PUBLIC location
        //    string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
        //    string application = "";

        //    string extension = Path.GetExtension(filePath);
        //    switch (extension.ToLower())
        //    {
        //        case ".doc":
        //        case ".docx":
        //            application = "application/msword";
        //            break;
        //        case ".pdf":
        //            application = "application/pdf";
        //            break;
        //        case ".xls":
        //        case ".xlsx":
        //            application = "application/vnd.ms-excel";
        //            break;
        //        case ".jpg":
        //        case ".jpeg":
        //        case ".png":
        //            application = "image/jpeg";
        //            break;
        //        default:
        //            application = "*/*";
        //            break;
        //    }
        //    var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/invoices" + extension;
        //    File.WriteAllBytes(externalPath, bytes);

        //    var file = new Java.IO.File(externalPath);
        //    file.SetReadable(true);

        //    var uri = Android.Net.Uri.FromFile(file);
        //    var intent = new Intent(Intent.ActionView);
        //    intent.SetDataAndType(uri, application);
        //    intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

        //    try
        //    {
        //        GetApplicationContext().StartActivity(intent);
        //    }
        //    catch (Exception)
        //    {
        //        Android.Widget.Toast.MakeText(GetApplicationContext(), "No Application Available to View PDF", Android.Widget.ToastLength.Short).Show();
        //    }
        //}

        /// <inheritdoc />
        public override Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1)
        {
            fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.pdf";
            return InternalSave(htmlContent, fileName, kind, numberOfPages, (source, absolutePath) => new PdfVectorExportWebViewCallBack(source, absolutePath, numberOfPages, new PaperSize(kind)));
        }

        /// <inheritdoc />
        public override Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1)
        {
            fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.png";
            return InternalSave(htmlContent, fileName, kind, numberOfPages, (source, absolutePath) => new PngExportWebViewCallBack(source, absolutePath));
        }

        /// <inheritdoc />
        protected virtual Task<string> InternalSave(string htmlContent, string fileName, PaperKind kind, int numberOfPages, Func<TaskCompletionSource<string>, string, WebViewClient> webviewclientCreator)
        {
            var source = new TaskCompletionSource<string>();

            var file = System.IO.Path.Combine(global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, fileName);

            // AVOID: Java.Lang.RuntimeException: WebView cannot be initialized on a thread that has no Looper.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var context = GetApplicationContext();

                    WebView.EnableSlowWholeDocumentDraw();
                    var view = new global::Android.Webkit.WebView(context);
                    view.Settings.JavaScriptEnabled = true;
                    view.Settings.OffscreenPreRaster = true;
                    view.SetBackgroundColor(global::Android.Graphics.Color.White);
                    view.SetInitialScale(100); // density fix
                    view.SetPadding(0, 0, 0, 0);
                    view.LoadDataWithBaseURL("", htmlContent, "text/html", "UTF-8", null);
                    view.ClearCache(true);
                    view.Settings.UseWideViewPort = true;
                    view.Settings.LoadWithOverviewMode = true;
                    /*
                    int widthMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, global::Android.Views.MeasureSpecMode.Unspecified);
                    int heightMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, global::Android.Views.MeasureSpecMode.Unspecified);
                    view.Measure(widthMeasureSpec, heightMeasureSpec);
                    */
                    var size = new PaperSize(kind);
                    view.Layout(0, 0, size.Width, (size.Height == 0 ? size.Width /*fallback*/ : size.Height) * numberOfPages); // Experimental size for n A4 pages

                    view.SetWebViewClient(webviewclientCreator(source, file));
                }
                catch (Exception e) { source.SetException(e); }
            });

            return source.Task;
        }

        /// <inheritdoc />
        protected virtual global::Android.Content.Context GetApplicationContext() => MvvmCross.Mvx.IoCProvider.Resolve<MvvmCross.Platforms.Android.IMvxAndroidCurrentTopActivity>().Activity;
    }
}