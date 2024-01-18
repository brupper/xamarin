using Brupper.Push.Models;
using System;
using System.Threading.Tasks;

namespace Brupper.Push
{
    /// <summary> Creates local toast notifications </summary>
    public interface IToastNotificationService
    {
        /// <summary> Creates a platform specific toast notification. </summary>
        Task CreateToastAsync(Content message, Func<Task> tappedAction);
    }
}
