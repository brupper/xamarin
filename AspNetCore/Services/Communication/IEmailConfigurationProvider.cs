using Microsoft.Extensions.Configuration;

namespace Brupper.AspNetCore.Services.Communication;

public interface IEmailConfigurationProvider
{
    Task<EmailCommunicationServiceConfig> GetAsync();
    Task<EmailCommunicationServiceConfig?> GetTenantConfigurationAsync();
}

public class EmailConfigurationProvider(IBrandContextAccessor brandContextAccessor, IConfiguration configuration) : IEmailConfigurationProvider
{
    private readonly IBrandContextAccessor brandContextAccessor = brandContextAccessor;
    private readonly IConfiguration configuration = configuration;

    public async Task<EmailCommunicationServiceConfig> GetAsync()
    {
        await Task.CompletedTask;

        return configuration.GetSection("mail").Get<EmailCommunicationServiceConfig>()!; // ez egyelőre nem future proof.
    }

    public async Task<EmailCommunicationServiceConfig?> GetTenantConfigurationAsync()
    {
        var brandEmailConfiguration = await brandContextAccessor.GetBrandEmailConfigurationAsync();

        if (brandEmailConfiguration == null)
        {
            return null;
        }

        return new()
        {
            ConnectionString = brandEmailConfiguration.ConnectionString!,
            FromEmail = brandEmailConfiguration.FromEmail!,
            FromName = brandEmailConfiguration.FromName!,
        };
    }
}