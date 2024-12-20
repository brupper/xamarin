using Brupper.AspNetCore.Services.Communication;

namespace Brupper.AspNetCore;


public interface IBrandContextAccessor
{
    Task<string?> GetBrandIdAsync();

    Task<IEnumerable<string>> GetBrandIdsAsync();

    Task<EmailCommunicationServiceConfig> GetBrandEmailConfigurationAsync();
}

public interface IBrandContextAccessor<TEntity> : IBrandContextAccessor
{
    Task<TEntity?> GetBrandContextAsync();

    Task<IEnumerable<TEntity>> GetBrandsAsync();
}
