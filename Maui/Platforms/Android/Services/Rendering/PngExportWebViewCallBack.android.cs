using Android.Graphics;
using AndroidWebView = Android.Webkit.WebView;
using System;
using System.IO;
using System.Threading.Tasks;
using AndroidPaint = Android.Graphics.Paint;

namespace Brupper.Maui.Platforms.Android.Services;

// https://stackoverflow.com/questions/37340813/xamarin-free-html-or-doc-to-pdf-conversion
public class PngExportWebViewCallBack : global::Android.Webkit.WebViewClient
{
    private TaskCompletionSource<string> source;
    private string fileNameWithPath = null;
    private bool checkOnPageStartedCalled;

    public PngExportWebViewCallBack(TaskCompletionSource<string> source, string path)
    {
        this.source = source;
        fileNameWithPath = path;
    }

    public override void OnPageStarted(AndroidWebView view, string url, Bitmap favicon)
    {
        base.OnPageStarted(view, url, favicon);
        checkOnPageStartedCalled = true;
    }

    // https://forums.xamarin.com/discussion/174630/how-to-convert-webview-source-html-string-to-image-format
    public override async void OnPageFinished(AndroidWebView webview, string url)
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

            var bitmap = Bitmap.CreateBitmap(htmlSize.width, htmlSize.height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
            var paint = new AndroidPaint();
            canvas.DrawBitmap(bitmap, 0, bitmap.Height, paint);
            webview.Draw(canvas);

            var stream = new FileStream(fileNameWithPath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();

            source.SetResult(fileNameWithPath);
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            source.SetException(exception);
        }
    }
}
