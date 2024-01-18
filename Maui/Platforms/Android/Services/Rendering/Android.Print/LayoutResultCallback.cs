using Android.OS;
using Android.Runtime;
using Java.Lang;
using System.Threading.Tasks;

namespace Android.Print;

// javac.exe error JAVAC0000:  error: LayoutResultCallback() is not public in LayoutResultCallback; cannot be accessed from outside package
// javac.exe error JAVAC0000: public class PdfPrintCallback
//[Register("android/print/PrintDocumentAdapter$LayoutResultCallback", DoNotGenerateAcw = true)]
[Register("android/print/MyLayoutResultCallback")]
public class LayoutResultCallback : PrintDocumentAdapter.LayoutResultCallback
{
    PrintDocumentAdapter printAdapter;
    private readonly TaskCompletionSource<string> source;
    private readonly string fileResultPath;

    #region Constructor

    // JNIEnv.Handle, JniHandleOwnership.DoNotRegister
    // IntPtr.Zero, JniHandleOwnership.DoNotTransfer
    public LayoutResultCallback(PrintDocumentAdapter printAdapter, TaskCompletionSource<string> source, string fileResultPath)
    //    : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
    //    : base(JNIEnv.Handle, JniHandleOwnership.DoNotRegister)
        : base(JNIEnv.AllocObject(typeof(LayoutResultCallback)), JniHandleOwnership.TransferLocalRef)
    {
        this.printAdapter = printAdapter;
        this.source = source;
        this.fileResultPath = fileResultPath;
    }

    // public PdfPrintCallback(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

    #endregion

    protected override void Dispose(bool disposing)
    {
        //base.Dispose(disposing);
    }

    public override void OnLayoutFailed(ICharSequence error)
    {
        //base.OnLayoutFailed(error);
    }

    public override void OnLayoutCancelled()
    {
        //base.OnLayoutCancelled();
    }

    public override void OnLayoutFinished(PrintDocumentInfo info, bool changed)
    {
        //base.OnLayoutFinished(info, changed);
        var fd = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(fileResultPath), ParcelFileMode.Create | ParcelFileMode.ReadWrite);
        printAdapter.OnWrite(new[] { PageRange.AllPages }, fd, new CancellationSignal(), new WriteResultCallback(source, fileResultPath));
        //printAdapter.OnWrite(new[] { PageRange.AllPages }, fd, new CancellationSignal(), null);
    }
}
