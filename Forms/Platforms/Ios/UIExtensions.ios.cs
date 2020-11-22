using UIKit;

namespace Brupper.Forms.Platforms.iOS
{
    public static class UIExtensions
    {
        // MAKE SURE YOU ARE CALLING IT FROM MAIN THREAD
        public static (int width, int height) GetInnerSize(this UIWebView webView)
        {
            var jsrHeightResult = webView.EvaluateJavascript(ViewExtensions.JavaSciptGetWindowHeight).Replace("\"", "") ?? string.Empty; // ide WINDOW height kell --- document.documentElement.clientHeight
            var jsrWidthResult = webView.EvaluateJavascript(ViewExtensions.JavaSciptGetWidth).Replace("\"", "") ?? string.Empty;    // document.body.getBoundingClientRect()

            double height = 1d, width = 1d;
            if ((double.TryParse(jsrHeightResult, out height) || double.TryParse(jsrHeightResult.Replace('.', ','), out height))
                && (double.TryParse(jsrWidthResult, out width) || double.TryParse(jsrWidthResult.Replace('.', ','), out width)))
            {
                return ((int)width, (int)height);
            }

            return (0, 0);
        }

        public static UIColor FromHex(this UIColor color, int hexValue)
        {
            return UIColor.FromRGB(
                (((float)((hexValue & 0xFF0000) >> 16)) / 255.0f),
                (((float)((hexValue & 0xFF00) >> 8)) / 255.0f),
                (((float)(hexValue & 0xFF)) / 255.0f)
            );
        }
    }
}