using Brupper.AspNetCore.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Brupper.AspNetCore;

public static class BrupperModule
{
    public static void AddBrupper(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(BrupperModule).GetTypeInfo().Assembly;

        services.AddControllersWithViews()
            .AddApplicationPart(assembly) // viewcomponent miatt kell
            // .AddRazorRuntimeCompilation()
            ;

        // Program.cs: services.Configure<MvcRazorRuntimeCompilationOptions>(options => options.FileProviders.Add(new EmbeddedFileProvider(assembly)));


        services.RegistertRepositories();
        services.RegistertDatabase(configuration);
        services.RegistertServices();
        services.RegistertRemoteServices();
        services.RegistertMapper();

        services.WithMemoryCache();
    }

    private static void RegistertRepositories(this IServiceCollection services)
    {
    }

    private static void RegistertRemoteServices(this IServiceCollection services)
    {
    }

    private static void RegistertDatabase(this IServiceCollection services, IConfiguration configuration)
    {
    }

    private static void RegistertServices(this IServiceCollection services)
    {
    }

    private static void RegistertMapper(this IServiceCollection services)
    {
    }
}
