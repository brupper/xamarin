namespace Brupper.AspNetCore.Services.Communication;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class FileAttachment
{
    public const string OctetStream = "application/octet-stream";

    public string Base64Content { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public string FileName { get; set; } = default!;
}
