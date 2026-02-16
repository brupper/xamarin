using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer;

/// <summary> Uploads files to the server. </summary>
public interface IFileUploader
{
    /// <summary>
    /// Does the upload operation asynchronously.
    /// </summary>
    /// <param name="request">The background transfer request.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
    /// <returns>The response object which contains the result of the uploading.</returns>
    Task<UploadFinishedResponse> UploadAsync(BackgroundTransferRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Does the delete operation asynchronously.
    /// </summary>
    /// <param name="photoId">The photo ID.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the request.</param>
    /// <returns>The response object which contains the result of the deleting.</returns>
    Task<HttpResponseMessage> DeleteAsync(string photoId, CancellationToken cancellationToken);

    /// <summary> Sets the progress of the photo uploading. </summary>
    /// <param name="progress">The progress of the photo uploading.</param>
    void Attach(IProgress<double> progress);
}