using Microsoft.Extensions.DependencyInjection;

namespace Brupper.AspNetCore.Caching;

public static partial class CachingExtensions
{
    /// <summary> Register the ICacheService </summary>
    public static IServiceCollection WithMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}
