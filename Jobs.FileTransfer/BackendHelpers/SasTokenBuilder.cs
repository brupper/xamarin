using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Brupper.Jobs.FileTransfer.Azure;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.BackendHelpers;

public class SasTokenBuilder
{
    public async Task<SasResponse> GenerateAsync(BlobServiceClient blobServiceClient)
    {
        var response = new SasResponse();
        try
        {
            // https://learn.microsoft.com/en-us/azure/storage/blobs/sas-service-create-dotnet
            // var blobServiceClient = new BlobServiceClient(options.Value.Secret);

            // Check if BlobContainerClient object has been authorized with Shared Key
            if (blobServiceClient.CanGenerateAccountSasUri)
            {
                var ttl = DateTimeOffset.UtcNow.AddDays(7);

                // Create a SAS token that's valid for 7 days
                var sasUri = blobServiceClient.GenerateAccountSasUri(
                    AccountSasPermissions.Create | AccountSasPermissions.Write | AccountSasPermissions.List,
                    ttl,
                    AccountSasResourceTypes.Object | AccountSasResourceTypes.Container
                    );

                response = new()
                {
                    Token = sasUri.ToString(),
                    Ttl = Convert.ToInt32((DateTimeOffset.UtcNow.AddDays(7) - DateTimeOffset.UtcNow).TotalSeconds)
                };
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);

            response = new();
            // response.Exception = new ServerException { ServerMessage = exception.ToString() };
        }

        return response;
    }
}
