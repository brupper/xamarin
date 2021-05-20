using Android.OS;
using Android.Print;
using Android.Runtime;
using Java.Lang;
using System;

namespace Android.Print
{
    // javac.exe error JAVAC0000:  error: LayoutResultCallback() is not public in LayoutResultCallback; cannot be accessed from outside package
    // javac.exe error JAVAC0000: public class PdfPrintCallback
    //[Register("android/print/PrintDocumentAdapter$LayoutResultCallback", DoNotGenerateAcw = true)]
    [Register("android/print/MyLayoutResultCallback")]
    public class LayoutResultCallback : PrintDocumentAdapter.LayoutResultCallback
    {
        PrintDocumentAdapter printAdapter;
        string fileNameWithPath;

        #region Constructor

        // JNIEnv.Handle, JniHandleOwnership.DoNotRegister
        // IntPtr.Zero, JniHandleOwnership.DoNotTransfer
        public LayoutResultCallback(PrintDocumentAdapter printAdapter, string fileNameWithPath)
        //    : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
        //    : base(JNIEnv.Handle, JniHandleOwnership.DoNotRegister)
            : base(JNIEnv.AllocObject(typeof(LayoutResultCallback)), JniHandleOwnership.TransferLocalRef)
        {
            this.printAdapter = printAdapter;
            this.fileNameWithPath = fileNameWithPath;
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
            var fd = Android.OS.ParcelFileDescriptor.Open(new Java.IO.File(fileNameWithPath), ParcelFileMode.Create | ParcelFileMode.ReadWrite);
            printAdapter.OnWrite(new[] { PageRange.AllPages }, fd, new CancellationSignal(), new WriteResultCallback());
            //printAdapter.OnWrite(new[] { PageRange.AllPages }, fd, new CancellationSignal(), null);
        }
    }
}
