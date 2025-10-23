using Brupper.AspNetCore.Identity.Contexts;
using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Filters;
using Brupper.AspNetCore.Identity.MapperProfiles;
using Brupper.AspNetCore.Identity.Models;
using Brupper.AspNetCore.Identity.Permission;
using Brupper.AspNetCore.Identity.Repositories;
using Brupper.AspNetCore.Identity.Services;
using Brupper.AspNetCore.Identity.Services.Communication;
using Brupper.AspNetCore.Identity.Services.Users;
using Brupper.AspNetCore.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using IdentityConstants = Brupper.AspNetCore.Identity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity;

public static class Module
{
    public static IEndpointRouteBuilder MapIdentityManagementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        //endpoints
        //    .MapGroup("/api")
        //    .MapInfoUrl<User>()
        //    .MapCustomIdentityApi<User>();

        endpoints
            .MapGroup("/api")
            .MapControllerRoute(
                name: AreaConstants.AreaName,
                pattern: "{area:exists}/{controller=home}/{action=index}/{id?}");


        return endpoints;
    }

    public static async Task InitIdentityDatabaseAsync(this IServiceCollection services)
    {
        using var sp = services.BuildServiceProvider();
        using var sc = sp.CreateScope();

        await DefaultUsers.InitAndSeedDatabaseAsync(sc.ServiceProvider);
    }

    public static TBuilder AddIdentityCustomAdministration<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddIdentityCustomAdministration(builder.Configuration);

        return builder;
    }

    public static void AddIdentityCustomAdministration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
            options.AddPolicy(IdentityConstants.AuthorizationPolicy, policy => policy.RequireAuthenticatedUser()  /*jelenleg nincs korlatozva a menupont, mivel egy cegen belül mindenki menedzselheti a usereit. */ );
            // majd igy kapcsold vissza => options.AddPolicy(IdentityConstants.AuthorizationPolicy, policy => policy.RequireRole(IdentityConstants.Roles.SuperAdmin, IdentityConstants.Roles.TenantAdmin) );
            options.AddPolicy(IdentityConstants.Roles.SuperAdmin, policy => policy.RequireRole(IdentityConstants.Roles.SuperAdmin));
        });

        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IUserContextAccessor, UserNameContextAccessor>(); // singleton volt.

        services.AddCommunication(configuration);
        services.RegisterIdentity(configuration);
        services.RegisterRepositories(configuration);

        services.RegistertMapper();

#if DEBUG
        // This should be removed in production and the authority url changed to https
        // Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif
    }

    private static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            // enables immediate logout, after updating the user's stat.
            options.ValidationInterval = TimeSpan.Zero;
        });

        services.Configure<DataProtectionTokenProviderOptions>(o =>
        {
            // The default token life span is 1 day.
            // Set token life span to 5 days
            o.TokenLifespan = TimeSpan.FromDays(5); // set password reset token lifetime 
        });

        services.AddScoped<TokenUrlEncoderService>();

        //Microsoft.AspNetCore.Authorization.DefaultAuthorizationService
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, AuthorizationHandler>();
    }

    private static void RegisterContextOptions<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
        => services.AddScoped(sp =>
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<TContext>()
            ;
#if DEBUG
            // /*
            contextOptionsBuilder.EnableSensitiveDataLogging(true);
            contextOptionsBuilder.LogTo(Console.WriteLine);
            contextOptionsBuilder.LogTo(text => System.Diagnostics.Debug.WriteLine(text));
            // */
#endif

            return contextOptionsBuilder.Options;
        });

    private static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterContextOptions<TenantDataContext>(configuration);
        services.AddDbContext<TenantDataContext>();

        services.AddScoped<ITenantRepository, TenantRepository>();
    }

    private static void AddCommunication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddScoped<IUrlHelper>(x =>
        {
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext!;
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

    private static void RegistertMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToViewModelMappingProfile));
    }
}
