using System.Net;

namespace Brupper.Jobs.FileTransfer;

public class UploadFinishedResponse
{
    public HttpStatusCode HttpStatusCode { get; set; }

    public string HttpStatusMessage { get; set; }

    public string Content { get; set; }
}
