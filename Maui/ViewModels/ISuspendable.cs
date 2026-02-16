using System;

namespace Brupper.Maui.ViewModels;

public interface ISuspendable
{
    bool IsSuspended { get; }

    IDisposable BeginSuspend();

    void EndSuspend();
}
