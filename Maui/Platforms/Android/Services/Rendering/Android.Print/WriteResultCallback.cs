using Android.Runtime;
using System.Threading.Tasks;

namespace Android.Print;

[Register("android/print/MyWriteResultCallback")]
//[Register("android/print/PrintDocumentAdapter$WriteResultCallback", DoNotGenerateAcw = true)]
public class WriteResultCallback : PrintDocumentAdapter.WriteResultCallback
{
    private readonly TaskCompletionSource<string> source;
    private readonly string fileResultPath;

    public WriteResultCallback(TaskCompletionSource<string> source, string fileResultPath)
    //    : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
    //    : base(JNIEnv.Handle, JniHandleOwnership.DoNotRegister)
        : base(JNIEnv.AllocObject(typeof(WriteResultCallback)), JniHandleOwnership.TransferLocalRef)
    {
        this.source = source;
        this.fileResultPath = fileResultPath;
    }

    public override void OnWriteFinished(PageRange[] pages)
    {
        // DO NOT: base.OnWriteFinished(pages);
        source.SetResult(fileResultPath);
    }
}
