using Azure;
using Azure.Storage.Blobs;
using System;

namespace Brupper.Jobs.FileTransfer.Azure;

public interface IFileProxyServiceFactory
{
    IBlobStorageService CreateFromSasToken(string sasToken);
}

public class FileProxyServiceFactory : IFileProxyServiceFactory
{
    public IBlobStorageService CreateFromSasToken(string sasToken)
    {
        var uri = new Uri(sasToken);
        var blobUriBuilder = new BlobUriBuilder(uri);
        var cred = new AzureSasCredential(blobUriBuilder.Sas.ToString());

        blobUriBuilder.Sas = null;
        var service = new global::Azure.Storage.Blobs.BlobServiceClient(blobUriBuilder.ToUri(), cred);

        //var service = new global::Azure.Storage.Blobs.BlobServiceClient(sasToken);
        return new AzureBlobService(service);
    }
}
