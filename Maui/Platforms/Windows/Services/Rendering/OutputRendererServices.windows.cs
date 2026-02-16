using Brupper.Maui.Models.Rendering;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Maui.Platforms.Windows.Services;

public class OutputRendererServices : Forms.Services.Concretes.AOutputRendererServices
{
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

    public override Task<string> SaveIntoPdfAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1)
    {
        fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.pdf";
        //return InternalSave(htmlContent, fileName, kind, numberOfPages, (source, absolutePath) => new PdfExportWebViewCallBack(source, absolutePath, numberOfPages, new PaperSize(kind)));

        return Task.FromResult(fileName);
    }

    public override Task<string> SaveIntoPngAsync(string htmlContent, string fileName, PaperKind kind, int numberOfPages = 1)
    {
        fileName = $"{DateTime.Now:yyyyMMdd}_{fileName}.png";
        //return InternalSave(htmlContent, fileName, kind, numberOfPages, (source, absolutePath) => new PngExportWebViewCallBack(source, absolutePath));

        return Task.FromResult(fileName);
    }

    //protected virtual Task<string> InternalSave(string htmlContent, string fileName, PaperKind kind, int numberOfPages, Func<TaskCompletionSource<string>, string, WebViewClient> webviewclientCreator)
    //{
    //    var source = new TaskCompletionSource<string>();

    //    var file = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);

    //    // AVOID: Java.Lang.RuntimeException: WebView cannot be initialized on a thread that has no Looper.
    //    MainThread.BeginInvokeOnMainThread(() => { try { } catch (Exception e) { source.SetException(e); } });

    //    return source.Task;
    //}
}