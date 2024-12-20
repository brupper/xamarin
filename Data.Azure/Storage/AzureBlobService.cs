using Azure.Storage.Blobs;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// https://www.intertech.com/asp-net-core-azure-blob-storage-repository-pattern
namespace Brupper.Data.Azure;

public interface IBlobStorageService
{
    Task DeleteDocumentAsync(string blobName, CancellationToken cancellationToken = default);
    Task<Stream> GetBlobContentAsync(string blobName, CancellationToken cancellationToken = default);
    Task UpdateBlobContentAsync(string blobName, Stream content, CancellationToken cancellationToken = default);
    Task<List<string>?> GetListOfBlobsInFolderAsync(string folderName, CancellationToken cancellationToken = default);
}

public class AzureBlobService(
    BlobServiceClient storageClient
        ) : IBlobStorageService
{
    public const string StorageContainerName = "parts"; // TODO: config

    private readonly BlobServiceClient storageClient = storageClient;

    #region Constructor

    #endregion

    #region interface implementation

    /// <inheritdoc/>
    public async Task DeleteDocumentAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Stream> GetBlobContentAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = await GetContainerClientAsync(cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<string>?> GetListOfBlobsInFolderAsync(string folderName, CancellationToken cancellationToken = default)
    {
        var results = new List<string>();

        var containerClient = await GetContainerClientAsync(cancellationToken);

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
        var containerClient = await GetContainerClientAsync(cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(content, true, cancellationToken);
    }

    #endregion

    private async Task<BlobContainerClient> GetContainerClientAsync(CancellationToken cancellationToken = default)
    {
        var blobContainerClient = storageClient.GetBlobContainerClient(StorageContainerName);

        await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        return blobContainerClient;
    }
}
