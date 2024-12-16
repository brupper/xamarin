using System;

namespace Brupper.Jobs.FileTransfer;

public class UploadTransferRequest
{
    #region Public Properties

    public QueryParameterCollection Parameters { get; set; }

    public Uri Uri { get; private set; }

    public string Content { get; private set; }

    public QueryParameterCollection Headers { get; set; }

    #endregion

    #region Constructors

    public UploadTransferRequest(Uri uri, string content)
    {
        Uri = uri;
        Parameters = new QueryParameterCollection();
        Content = content;
        Headers = new QueryParameterCollection();
    }

    #endregion
}
