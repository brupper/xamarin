using Android.Webkit;
using System.Threading.Tasks;

namespace Brupper.Maui.Platforms.Android;

public static class UIExtensions
{
    // MAKE SURE YOU ARE CALLING IT FROM MAIN THREAD
    public static async Task<(int width, int height)> GetInnerSize(this WebView webView)
    {
        var jsrHeight = new JavascriptResult();
        webView.EvaluateJavascript(ViewExtensions.JavaSciptGetWindowHeight, jsrHeight); // ide WINDOW height kell --- document.documentElement.clientHeight
        var jsrHeightResult = (await jsrHeight.JsResult)?.Replace("\"", "") ?? string.Empty;

        var jsrWidth = new JavascriptResult();
        webView.EvaluateJavascript(ViewExtensions.JavaSciptGetWidth, jsrWidth);    // document.body.getBoundingClientRect()
        var jsrWidthResult = (await jsrWidth.JsResult)?.Replace("\"", "") ?? string.Empty;

        double height, width;
        if ((double.TryParse(jsrHeightResult, out height) || double.TryParse(jsrHeightResult.Replace('.', ','), out height))
            && (double.TryParse(jsrWidthResult, out width) || double.TryParse(jsrWidthResult.Replace('.', ','), out width)))
        {
            return ((int)width, (int)height);
        }

        return (0, 0);
    }
}