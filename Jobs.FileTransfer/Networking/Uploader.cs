using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer;

public class ServiceRegistration
{
    public static ServiceRegistration Current = new();

    public string CurrentApiVersion { get; } = "v1";
}

internal class JsonContent(string json) : StringContent(json, Encoding.UTF8, "application/json") { }

/// <summary> <see cref="Brupper.Jobs.Interfaces.IUploader"/> implementation. </summary>
public class Uploader : IUploader
{
    #region Private Properties

    private readonly ILogger logger;
    private readonly IHttpClientFactory httpClientFactory;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Brupper.Jobs.Implementations.Uploader"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public Uploader(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        Guard.IsNotNull(httpClientFactory, nameof(httpClientFactory));
        Guard.IsNotNull(logger, nameof(logger));

        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    #endregion

    #region IUploader implementation 

    public async Task<UploadFinishedResponse> UploadAsync(UploadTransferRequest uploadRequest, CancellationToken cancellationToken)
    {
        using (var httpClient = httpClientFactory.CreateClient(ServiceRegistration.Current.CurrentApiVersion))
        {
            var contentToPost = new JsonContent(uploadRequest.Content);

            foreach (var header in uploadRequest.Headers)
            {
                contentToPost.Headers.Add(header.Key, header.Value);
            }

            try
            {
                var result = await httpClient.PostAsync(uploadRequest.Uri, contentToPost, cancellationToken);

                return new UploadFinishedResponse()
                {
                    Content = result.ToString(),
                    HttpStatusCode = HttpStatusCode.Created
                };
            }
            //catch (ApiException e)
            //{
            //    return new UploadFinishedResponse()
            //    {
            //        HttpStatusCode = e.HttpStatus,
            //        HttpStatusMessage = e.ServerMessage
            //    };
            //}
            catch (Exception e)
            {
                logger.LogError($"{e}");
                throw;
            }
        }
    }

    public async Task<UploadFinishedResponse> UpdateAsync(UploadTransferRequest uploadRequest, CancellationToken cancellationToken)
    {
        using (var httpClient = httpClientFactory.CreateClient(ServiceRegistration.Current.CurrentApiVersion))
        {
            var contentToPost = new JsonContent(uploadRequest.Content);

            foreach (var header in uploadRequest.Headers)
            {
                contentToPost.Headers.Add(header.Key, header.Value);
            }

            try
            {
                var result = await httpClient.PostAsync(uploadRequest.Uri, contentToPost, cancellationToken);

                return new UploadFinishedResponse()
                {
                    Content = result.ToString(),
                    HttpStatusCode = HttpStatusCode.NoContent
                };
            }
            //catch (ApiException e)
            //{
            //    return new UploadFinishedResponse()
            //    {
            //        HttpStatusCode = e.HttpStatus,
            //        HttpStatusMessage = e.ServerMessage
            //    };
            //}
            catch (Exception e)
            {
                logger.LogError($"{e}");
                throw;
            }
        }
    }

    #endregion
}
