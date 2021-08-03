using System;

namespace Brupper
{
    public interface ISuspendable
    {
        bool IsSuspended { get; }

        IDisposable BeginSuspend();

        void EndSuspend();
    }
}
