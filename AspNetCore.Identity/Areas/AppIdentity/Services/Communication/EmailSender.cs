using Brupper.AspNetCore.Services.Communication;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Communication;

public class EmailSender : IEmailSender
{
    private readonly IEmailService emailService;

    public EmailSender(IEmailService emailService) => this.emailService = emailService;

    public Task SendEmailAsync(string emailAddress, string subject, string htmlMessage)
        => emailService.SendEmailAsync(new() { Subject = subject, Body = htmlMessage, ToEmails = new() { new() { Email = emailAddress, Name = emailAddress } } });
}
