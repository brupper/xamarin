using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InspEx.Upload.FileTransfer;

public class ProgressStreamContent : StreamContent
{
    private readonly IProgress<double> progress;
    private readonly Stream content;
    private const int ProgressFrequency = 8;
    private int progressHelper;

    public ProgressStreamContent(Stream stream, IProgress<double> progress)
        : base(stream)
    {
        this.progress = progress;
        content = new MemoryStream();
        stream.CopyTo(content);
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        return Task.Run(() =>
        {
            var buffer = new byte[4096];
            var size = content.Length;

            if (content.CanSeek)
            {
                content.Position = 0;
            }

            int bytesRead;
            while ((bytesRead = content.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                var progressPercentage = (stream.Length / (double)content.Length) * 100;

                if (this.NeedToReportProgress(progressPercentage))
                {
                    progress.ReportProgress(progressPercentage);
                }
            }
        });
    }

    private bool NeedToReportProgress(double progressPercentageToCheck)
    {
        bool result = false;
        var normalizedProgress = (int)(progressPercentageToCheck / (100.0 / ProgressFrequency));
        if (progressHelper != normalizedProgress)
        {
            result = true;
            progressHelper = normalizedProgress;
        }

        return result;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = content.Length;
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            content?.Dispose();
        }

        base.Dispose(disposing);
    }
}