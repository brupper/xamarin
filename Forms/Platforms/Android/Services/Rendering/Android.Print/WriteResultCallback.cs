using Android.Runtime;
using System;

namespace Android.Print
{
    [Register("android/print/MyWriteResultCallback")]
    //[Register("android/print/PrintDocumentAdapter$WriteResultCallback", DoNotGenerateAcw = true)]
    public class WriteResultCallback : PrintDocumentAdapter.WriteResultCallback
    {
        public WriteResultCallback()
        //    : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
        //    : base(JNIEnv.Handle, JniHandleOwnership.DoNotRegister)
            : base(JNIEnv.AllocObject(typeof(WriteResultCallback)), JniHandleOwnership.TransferLocalRef)
        { }

        public override void OnWriteFinished(PageRange[] pages)
        {
            // DO NOT: base.OnWriteFinished(pages);
        }
    }
}
