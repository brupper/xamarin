using Azure.Storage.Blobs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// https://www.intertech.com/asp-net-core-azure-blob-storage-repository-pattern
namespace Brupper.Jobs.FileTransfer.Azure;

/// <summary>  </summary>
public interface IBlobStorageService
{
    /// <summary>  </summary>
    Task DeleteDocumentAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>  </summary>
    Task<Stream> GetBlobContentAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>  </summary>
    Task UpdateBlobContentAsync(string blobName, Stream content, CancellationToken cancellationToken = default);

    /// <summary>  </summary>
    Task<List<string>> GetListOfBlobsInFolderAsync(string folderName, CancellationToken cancellationToken = default);
}

public class AzureBlobService : IBlobStorageService
{
    private readonly BlobServiceClient storageClient;

    #region Constructor

    public AzureBlobService(
        BlobServiceClient storageClient
        )
    {
        this.storageClient = storageClient;
    }

    #endregion

    #region interface implementation

    /// <inheritdoc/>
    public async Task DeleteDocumentAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(GetContainerNameFromRelativeUri(blobName), cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Stream> GetBlobContentAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(GetContainerNameFromRelativeUri(blobName), cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<string>?> GetListOfBlobsInFolderAsync(string folderName, CancellationToken cancellationToken = default)
    {
        var results = new List<string>();

        var containerClient = await GetContainerClientAsync(GetContainerNameFromRelativeUri(folderName), cancellationToken);

        var blobsInFolder = containerClient.GetBlobs(prefix: folderName);

        if (blobsInFolder is not null)
        {
            foreach (var blob in blobsInFolder)
            {
                results.Add(blob.Name);
            }

            return results;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task UpdateBlobContentAsync(string blobName, Stream content, CancellationToken cancellationToken = default)
    {
        var containerName = GetContainerNameFromRelativeUri(blobName);
        var containerClient = await GetContainerClientAsync(containerName, cancellationToken);

        var blobNameWithoutContainerPrefix = blobName.Replace($"{containerName}/", "");//.Replace($"{containerName}\\","");
        var blobClient = containerClient.GetBlobClient(blobNameWithoutContainerPrefix);

        await blobClient.UploadAsync(content, true, cancellationToken);
    }

    #endregion

    private async Task<BlobContainerClient> GetContainerClientAsync(string container, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = storageClient.GetBlobContainerClient(container);

        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        return blobContainerClient;
    }

    private string GetContainerNameFromRelativeUri(string relativePath)
    {
        var path = relativePath;
        var urlSegments = path.Split('/');

        return urlSegments.First();
    }
}
