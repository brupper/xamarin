namespace Brupper.AspNetCore.Services.Communication;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest mailRequest);
}
