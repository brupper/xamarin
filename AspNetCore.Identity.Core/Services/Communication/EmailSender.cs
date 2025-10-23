using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Services.Communication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Brupper.AspNetCore.Identity.Services.Communication;

public class EmailSender(IEmailServiceFactory emailServiceFactory) : IEmailSender, IEmailSender<User>
{
    #region IEmailSender

    public async Task SendEmailAsync(string emailAddress, string subject, string htmlMessage)
    {
        var emailService = await emailServiceFactory.CreateForSystemAsync();
        await emailService.SendEmailAsync(new() { Subject = subject, Body = htmlMessage, ToEmails = new() { new() { Email = emailAddress, Name = emailAddress } } });
    }

    #endregion

    #region IEmailSender<User>

    internal const string Path = "Brupper.AspNetCore.Identity.Resources.MessageTemplates";
    internal const string ActivationTemplate = "AccountActivationMessage.html";
    internal const string PasswordRecoveryTemplate = "PasswordReminderMessage.html";

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var template = ActivationTemplate.GetEmbeddedResourceFromResourcesAsString(this.GetType().Assembly, Path);
        template = template.Replace("__confirmationEmailLink__", confirmationLink);

        await SendEmailAsync(email,
            "Fiók aktiválás",
            template);
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        => throw new NotImplementedException("SendPasswordResetCodeAsync is not implemented in Brupper.AspNetCore.Identity.Services.Communication.EmailSender<User>");

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var template = PasswordRecoveryTemplate.GetEmbeddedResourceFromResourcesAsString(this.GetType().Assembly, Path);
        template = template.Replace("__passwordResetLink__", resetLink);

        await SendEmailAsync(email,
            "Új jelszó beállítása",
            template);
    }

    #endregion
}
