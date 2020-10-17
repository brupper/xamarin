using Android.Webkit;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Forms.Platforms.Android.PlatformServices
{
    public class OutputRendererServices : Forms.Services.Concretes.AOutputRendererServices
    {
        public OutputRendererServices(IFileSystem fileSystem)
            : base(fileSystem)
        { }

        public override async Task OpenPdfAsync(string filePath)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath),
            });

            //var bytes = File.ReadAllBytes(filePath);

            ////Copy the private file's data to the EXTERNAL PUBLIC location
            //string externalStorageState = global::Android.OS.Environment.ExternalStorageState;
            //string application = "";

            //string extension = Path.GetExtension(filePath);
            //switch (extension.ToLower())
            //{
            //    case ".doc":
            //    case ".docx":
            //        application = "application/msword";
            //        break;
            //    case ".pdf":
            //        application = "application/pdf";
            //        break;
            //    case ".xls":
            //    case ".xlsx":
            //        application = "application/vnd.ms-excel";
            //        break;
            //    case ".jpg":
            //    case ".jpeg":
            //    case ".png":
            //        application = "image/jpeg";
            //        break;
            //    default:
            //        application = "*/*";
            //        break;
            //}
            //var externalPath = global::Android.OS.Environment.ExternalStorageDirectory.Path + "/invoices" + extension;
            //File.WriteAllBytes(externalPath, bytes);

            //var file = new Java.IO.File(externalPath);
            //file.SetReadable(true);

            //var uri = Android.Net.Uri.FromFile(file);
            //var intent = new Intent(Intent.ActionView);
            //intent.SetDataAndType(uri, application);
            //intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);

            //try
            //{
            //    GetApplicationContext().StartActivity(intent);
            //}
            //catch (Exception)
            //{
            //    Android.Widget.Toast.MakeText(GetApplicationContext(), "No Application Available to View PDF", Android.Widget.ToastLength.Short).Show();
            //}
        }

        public override Task<string> SaveIntoPdfAsync(string htmlContent, string fileName)
        {
            fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.pdf";
            return InternalSave(htmlContent, fileName, (source, absolutePath) => new PdfExportWebViewCallBack(source, absolutePath));
        }

        public override Task<string> SaveIntoPngAsync(string htmlContent, string fileName)
        {
            fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.png";
            return InternalSave(htmlContent, fileName, (source, absolutePath) => new PngExportWebViewCallBack(source, absolutePath));
        }


        private Task<string> InternalSave(string htmlContent, string fileName, Func<TaskCompletionSource<string>, string, WebViewClient> webviewclientCreator)
        {
            var source = new TaskCompletionSource<string>();

            //var file = Path.Combine(DocumentsFolder, fileName);
            var file = System.IO.Path.Combine(global::Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath, fileName);

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

            int widthMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, global::Android.Views.MeasureSpecMode.Unspecified);
            int heightMeasureSpec = global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, global::Android.Views.MeasureSpecMode.Unspecified);
            view.Measure(widthMeasureSpec, heightMeasureSpec);
            //view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);
            view.Layout(0, 0, 200, 200); // minel kisebb annal jobb, a magassag szelesssege ugy is a EvaluateJavascript altal vissza adott ertekek hatarozzak meg

            view.SetWebViewClient(webviewclientCreator(source, file));

            return source.Task;
        }

        private global::Android.Content.Context GetApplicationContext()
        {
            return MvvmCross.Mvx.IoCProvider.Resolve<MvvmCross.Platforms.Android.IMvxAndroidCurrentTopActivity>().Activity;
        }
    }
}