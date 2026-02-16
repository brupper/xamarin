using Microsoft.Maui.ApplicationModel.Communication;
using System.Collections.Generic;

namespace Brupper.Maui.Diagnostics;

public class ComposedEmail
{
    public IEnumerable<string> To { get; set; }
    public IEnumerable<string> Cc { get; set; } = null;
    public string Subject { get; set; } = null;
    public string Body { get; set; } = null;
    public bool IsHtml { get; set; } = false;
    public IEnumerable<EmailAttachment> Attachments { get; set; } = null;
    public string DialogTitle { get; set; } = null;

    public ComposedEmail(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body, bool isHtml, IEnumerable<EmailAttachment> attachments, string dialogTitle)
    {
        To = to;
        Cc = cc;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
        Attachments = attachments;
        DialogTitle = dialogTitle;
    }
}
