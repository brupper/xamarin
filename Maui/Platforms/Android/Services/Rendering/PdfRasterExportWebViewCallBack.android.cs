using Android.Graphics;
using Android.Graphics.Pdf;
using Android.Webkit;
using Android.Widget;
using Brupper.Maui.Models.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Brupper.Maui.Platforms.Android.Services;

// https://stackoverflow.com/questions/37340813/xamarin-free-html-or-doc-to-pdf-conversion
/// <summary>
/// Raszteres rendereléshez. Ezt leváltotta a vektorgrafikus.
/// </summary>
public class PdfRasterExportWebViewCallBack : WebViewClient
{
    private TaskCompletionSource<string> source;
    private string fileNameWithPath;
    private bool checkOnPageStartedCalled;
    private int numberOfPages;
    private PaperSize paperSize;

    public PdfRasterExportWebViewCallBack(TaskCompletionSource<string> source, string path, int numberOfPages, PaperSize pageSize)
    {
        this.source = source;
        fileNameWithPath = path;
        this.numberOfPages = numberOfPages;
        this.paperSize = pageSize;
    }

    public override void OnPageStarted(global::Android.Webkit.WebView view, string url, Bitmap favicon)
    {
        base.OnPageStarted(view, url, favicon);
        checkOnPageStartedCalled = true;
    }

    public override async void OnPageFinished(global::Android.Webkit.WebView webview, string url)
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
}