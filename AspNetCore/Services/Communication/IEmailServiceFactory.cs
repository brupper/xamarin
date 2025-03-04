namespace Brupper.AspNetCore.Services.Communication;

public interface IEmailServiceFactory
{
    Task<IEmailService> CreateForSystemAsync();
    Task<IEmailService> CreateByBrandAsync(string brandId);
}
