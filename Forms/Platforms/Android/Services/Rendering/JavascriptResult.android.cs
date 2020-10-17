using Android.Webkit;
using System.Threading.Tasks;

namespace Brupper.Forms.Platforms.Android
{
    class JavascriptResult : Java.Lang.Object, IValueCallback
    {
        TaskCompletionSource<string> source;
        public Task<string> JsResult { get { return source.Task; } }

        public JavascriptResult()
        {
            source = new TaskCompletionSource<string>();
        }

        public void OnReceiveValue(Java.Lang.Object result)
        {
            string json = ((Java.Lang.String)result).ToString();
            source.SetResult(json);
        }
    }
}