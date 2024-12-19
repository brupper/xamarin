using Brupper.AspNetCore.Services.Communication;

namespace Brupper.AspNetCore;

public interface IBrandContextAccessor
{
    Task<string?> GetBrandIdAsync();

    Task<IEnumerable<string>> GetBrandIdsAsync();

    Task<EmailCommunicationServiceConfig> GetBrandEmailConfigurationAsync();
}
