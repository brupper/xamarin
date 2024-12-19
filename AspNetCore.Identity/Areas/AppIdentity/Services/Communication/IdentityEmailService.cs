using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Communication;

public class IdentityEmailService<TUser>(
        IEmailSender sender,
        UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        LinkGenerator generator,        
        TokenUrlEncoderService encoder
    )
    : IEmailSender<TUser> 
        where TUser : class
{
    internal const string Path = "Brupper.AspNetCore.Identity.Resources.MessageTemplates";
    internal const string ActivationTemplate = "AccountActivationMessage.html";
    internal const string PasswordRecoveryTemplate = "PasswordReminderMessage.html";

    #region Properties

    public IEmailSender EmailSender { get; } = sender;
    public UserManager<TUser> UserManager { get; } = userManager;
    public IHttpContextAccessor ContextAccessor { get; } = contextAccessor;
    public LinkGenerator LinkGenerator { get; } = generator;
    public TokenUrlEncoderService TokenEncoder { get; } = encoder;

    #endregion

    private string GetUrl(string emailAddress, string token, string page)
    {
        var safeToken = TokenEncoder.EncodeToken(token);
        //var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        //return Url.Page(page, pageHandler: null, values: new { email = emailAddress, code = safeToken }, protocol: ContextAccessor.HttpContext.Request.Scheme);
        //return LinkGenerator.GetUriByPage(ContextAccessor.HttpContext, page, null, new { email = emailAddress, token = safeToken }) ?? string.Empty;
        var req = ContextAccessor.HttpContext.Request;
        return $"{req.Scheme}://{req.Host}{req.PathBase}/{page}?email={emailAddress}&code={safeToken}";
    }

    public async Task SendPasswordRecoveryEmail(string email, string confirmationPage)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null
            // || !(await UserManager.IsEmailConfirmedAsync(user))
            )
        {
            return;
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var token = await UserManager.GeneratePasswordResetTokenAsync(user);

        var url = GetUrl(email, token, confirmationPage); // HtmlEncoder.Default.Encode(callbackUrl)

        var template = PasswordRecoveryTemplate.GetEmbeddedResourceFromResourcesAsString(this.GetType().Assembly, Path);
        template = template.Replace("__passwordResetLink__", url);

        await EmailSender.SendEmailAsync(email,
            "[BRAND] új jelszó beállítása",
            template);
    }

    /// <inheritdoc/>
    public async Task SendConfirmationLinkAsync(TUser userReference, string email, string confirmationPage)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null
            // || !(await UserManager.IsEmailConfirmedAsync(user))
            )
        {
            return;
        }

        //var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        var token = await UserManager.GeneratePasswordResetTokenAsync(user); // password reset!

        var url = GetUrl(email, token, confirmationPage);

        var template = ActivationTemplate.GetEmbeddedResourceFromResourcesAsString(this.GetType().Assembly, Path);
        template = template.Replace("__confirmationEmailLink__", url);

        await EmailSender.SendEmailAsync(email,
            "[BRAND] fiók aktiválás",
            template);
    }

    /// <inheritdoc/>
    public Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
        => SendPasswordRecoveryEmail(email, "Identity/Account/ResetPassword");

    /// <inheritdoc/>
    public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
        => SendPasswordRecoveryEmail(email, "Identity/Account/ResetPassword");
}

