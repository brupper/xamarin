using System;

namespace Brupper.Jobs.FileTransfer;

/// <summary> . </summary>
public class BackgroundTransferRequest
{
    /// <summary> Initializes a new instance of the <see cref="BackgroundTransferRequest"/> class. </summary>
    /// <param name="uri">The URI to which the file will be uploaded.</param>
    /// <param name="fileName">The file name that will be sent to the server.</param>
    /// <param name="localPath">The local path of the file to upload.</param>
    public BackgroundTransferRequest(Uri uri, string fileName, string localPath)
    {
        Uri = uri;
        Parameters = new QueryParameterCollection();
        Method = "POST";
        FileName = fileName;
        LocalPath = localPath;
        Headers = new QueryParameterCollection();
    }

    /// <summary> Gets or sets the HTTP method used for the upload. The default method used is POST. </summary>
    public string Method { get; set; }

    /// <summary> Gets a list of parameters that should be sent as multi-part data with the uploaded file. </summary>
    public QueryParameterCollection Parameters { get; private set; }

    /// <summary> Gets the URI to which the file will be uploaded. </summary>
    public Uri Uri { get; private set; }

    /// <summary> Gets file name that will be sent to the server. </summary>
    public string FileName { get; private set; }

    /// <summary> Gets local path of the file to upload. </summary>
    public string LocalPath { get; private set; }

    /// <summary> Gets or sets an object that can contain state information related to the current transfer request. </summary>
    public object State { get; set; }

    /// <summary> . </summary>
    public QueryParameterCollection Headers { get; private set; }
}
