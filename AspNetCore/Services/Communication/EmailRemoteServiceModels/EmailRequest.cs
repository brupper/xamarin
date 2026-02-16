namespace Brupper.AspNetCore.Services.Communication;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EmailRequest
{
    public EmailAddressModel? From { get; set; }

    public List<EmailAddressModel> ToEmails { get; set; } = new();

    public string Subject { get; set; } = default!;

    public string Body { get; set; } = default!;

    public List<FileAttachment> Attachments { get; set; } = new();
}
