using CommunityToolkit.Diagnostics;
using InspEx.Upload.FileTransfer;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer;

/// <summary> <see cref="Brupper.Jobs.Interfaces.IFileUploader"/> implementation. </summary>
public class FileUploader : IFileUploader
{
    #region Private Properties

    private readonly IHttpClientFactory httpClientFactory;

    private IProgress<double> attachedProgress;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Brupper.Jobs.Implementations.FileUploader"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="storageManager">The storage manager.</param>
    public FileUploader(IHttpClientFactory httpClientFactory)
    {
        Guard.IsNotNull(httpClientFactory, nameof(httpClientFactory));

        this.httpClientFactory = httpClientFactory;
    }

    #endregion

    #region IFileUploader implementation

    public async Task<UploadFinishedResponse> UploadAsync(BackgroundTransferRequest request, CancellationToken cancellationToken)
    {
        using (var httpClient = httpClientFactory.CreateClient(ServiceRegistration.Current.CurrentApiVersion))
        {
            var content = await CreateContent(request);
            var response = await httpClient.PostAsync(new Uri(request.Uri.PathAndQuery, UriKind.Relative), content, cancellationToken);
            return await GetResultAsync(response);
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string photoId, CancellationToken cancellationToken)
    {
        using (var httpClient = httpClientFactory.CreateClient(ServiceRegistration.Current.CurrentApiVersion))
        {
            var uri = new Uri($"/public/v3/photos/{photoId}", UriKind.Relative);
            return await httpClient.DeleteAsync(uri, cancellationToken);
        }
    }

    public void Attach(IProgress<double> progress)
    {
        attachedProgress = progress;
    }

    #endregion

    #region Private Methods

    private async Task<HttpContent> CreateContent(BackgroundTransferRequest request)
    {
        var content = new MultipartFormDataContent();

        foreach (var parameter in request.Parameters)
        {
            content.Add(new StringContent(parameter.Value), string.Format($"\"{parameter.Key}\""));
        }

        if (!string.IsNullOrWhiteSpace(request.LocalPath))
        {
            var file = new MemoryStream(await File.ReadAllBytesAsync(request.LocalPath));
            file.Position = 0;
            var fileContent = new ProgressStreamContent(file, attachedProgress);
            content.Add(fileContent, "\"file\"", string.Format($"\"{request.FileName}\""));
        }

        return content;
    }

    private async Task<UploadFinishedResponse> GetResultAsync(HttpResponseMessage response)
    {
        if (response == null)
        {
            throw new Exception("Upload failed!");
        }

        var result = new UploadFinishedResponse();

        result.HttpStatusCode = response.StatusCode;
        result.HttpStatusMessage = response.ReasonPhrase;
        result.Content = await response.Content.ReadAsStringAsync();

        return result;
    }

    #endregion
}
