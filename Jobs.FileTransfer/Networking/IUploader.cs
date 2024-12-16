using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer;

/// <summary> Uploads data to the server. </summary>
public interface IUploader
{
    /// <summary> Does the upload operation asynchronously. </summary>
    /// <param name="uploadRequest">The upload transfer request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
    /// <returns>The response object which contains the result of the uploading.</returns>
    Task<UploadFinishedResponse> UploadAsync(UploadTransferRequest uploadRequest, CancellationToken cancellationToken);

    /// <summary> Does the upload operation asynchronously. </summary>
    /// <param name="uploadRequest">The upload transfer request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
    /// <returns>The response object which contains the result of the uploading.</returns>
    Task<UploadFinishedResponse> UpdateAsync(UploadTransferRequest uploadRequest, CancellationToken cancellationToken);
}
