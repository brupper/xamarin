using Brupper.Data.RemoteServices.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data.RemoteServices
{
    public interface IRemoteService
    {
        Task<StatusResponse> GetApiStatusAsync(CancellationToken cancellationToken = default);
    }
}
