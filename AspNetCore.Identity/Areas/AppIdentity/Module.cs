using Administration.Identity.Areas.AppIdentity;
using AspNetCore.Identity.CosmosDb.Extensions;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Contexts;
using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Repositories;
using Brupper.AspNetCore.Identity.Services.Communication;
using Brupper.AspNetCore.Services.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity;

public static class Module
{
    public static IEndpointRouteBuilder MapAppIdentityArea(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGroup("/api")
            .MapInfoUrl<User>()
            .MapCustomIdentityApi<User>();

        endpoints.MapControllerRoute(
                name: AreaConstants.AreaName,
                pattern: "{area:exists}/{controller=home}/{action=index}/{id?}");

        return endpoints;
    }

    public static void AddIdentityAdministration(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Module).GetTypeInfo().Assembly;

        services.AddControllersWithViews()
            .AddApplicationPart(assembly)
            .AddRazorRuntimeCompilation();

        services.Configure<MvcRazorRuntimeCompilationOptions>(options => options.FileProviders.Add(new EmbeddedFileProvider(assembly)));
        services.AddIdentityCustomAdministration(configuration);

        services.AddAuthentication();

        services.AddCommunication(configuration);
        services.RegisterIdentity(configuration);
        services.RegisterRepositories(configuration);

#if DEBUG
        // This should be removed in production and the authority url changed to https
        // Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif
    }

    private static void RegisterContextOptions<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
        => services.AddScoped(sp =>
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<TContext>()
            .UseCosmos(
                configuration["cosmosidentity:accountEndpoint"],
                configuration["cosmosidentity:accountKey"],
                configuration["cosmosidentity:databaseName"]
            // cosmosOptionsAction: opt => { opt.ser }
            );

#if DEBUG
            /*
            contextOptionsBuilder.EnableSensitiveDataLogging(true);
            contextOptionsBuilder.LogTo(Console.WriteLine);
            contextOptionsBuilder.LogTo(text => System.Diagnostics.Debug.WriteLine(text));
            // */
#endif

            return contextOptionsBuilder.Options;
        });

    private static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterContextOptions<IdentityDataContext>(configuration);

        services.AddDbContext<IdentityDataContext>();

        //services.AddIdentityApiEndpoints<User>(); //duplicate scheme Microsoft.AspNetCore.Identity.IdentityConstants.BearerAndApplicationScheme
        //TODO: services.AddCosmosIdentityCore<IdentityDataContext, User, IdentityRole>(
        services.AddCosmosIdentity<IdentityDataContext, User, IdentityRole, string>(
            opts =>
            {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.SignIn.RequireConfirmedAccount = false;
                opts.SignIn.RequireConfirmedEmail = false;
            },
            cookieExpireTimeSpan: TimeSpan.FromDays(7),
            slidingExpiration: true
        )

            // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityBuilderExtensions.cs#L94
            .AddApiEndpoints() // Adds configuration and services needed to support <see cref="IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi{TUser}(IEndpointRouteBuilder)"/>

            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            ;
    }

    private static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterContextOptions<CosmosTenantDataContext>(configuration);
        services.AddDbContext<CosmosTenantDataContext>();
        services.AddDbContext<TenantDataContext, CosmosTenantDataContext>();

        services.AddScoped<ITenantRepository, TenantRepository>();
    }

    private static void AddCommunication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        });
        services.AddTransient<IdentityEmailService<User>>();

        // services.AddScoped<IEmailSender, ConsoleEmailSender>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<Microsoft.AspNetCore.Identity.IEmailSender<User>, EmailSender>();  // ... .MapIdentityApi<>() needs transient services

        var buildSettings = new Func<IConfiguration, EmailCommunicationServiceConfig>(_ =>
        {
            var settings = _.GetSection("mail").Get<EmailCommunicationServiceConfig>()!;

            return settings;
        });
        // igy lenne a legszebb: services.AddOptions<CrmWebApiSettings>().Bind(configuration.GetSection("D365"));
        services.AddTransient<IOptions<EmailCommunicationServiceConfig>>(p => new OptionsWrapper<EmailCommunicationServiceConfig>(buildSettings(configuration)));
    }
}
