using AspNetCore.Identity.CosmosDb.Extensions;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.MapperProfiles;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Repositories;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Communication;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;
using Brupper.AspNetCore.Identity.Permission;
using Brupper.AspNetCore.Identity.Services;
using Microsoft.AspNetCore.Authorization;
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
using System.Reflection;
using IdentityConstants = Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users.IdentityConstants;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity;

public static class Module
{
    public static IEndpointRouteBuilder MapAppIdentityArea(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllerRoute(
                name: AreaConstants.AreaName,
                pattern: "{area:exists}/{controller=home}/{action=index}/{id?}");

        // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
        endpoints.MapGroup($"api/{AreaConstants.AreaName}").MapIdentityApiTEMP<User>();

        return endpoints;
    }

    public static async Task InitIdentityDatabaseAsync(this IServiceCollection services)
    {
        using var sp = services.BuildServiceProvider();
        using var sc = sp.CreateScope();

        await DefaultUsers.InitAndSeedDatabaseAsync(sc.ServiceProvider);
    }

    public static void AddAppIdentityAdministration(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(Module).GetTypeInfo().Assembly;

        services.AddControllersWithViews()
            .AddApplicationPart(assembly)
            .AddRazorRuntimeCompilation();

        services.Configure<MvcRazorRuntimeCompilationOptions>(options => options.FileProviders.Add(new EmbeddedFileProvider(assembly)));


        services.AddAuthentication();

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
            options.AddPolicy(IdentityConstants.AuthorizationPolicy, policy => policy.RequireRole(IdentityConstants.Roles.SuperAdmin, IdentityConstants.Roles.TenantAdmin));
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
            // /*
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
        services.AddCosmosIdentity<IdentityDataContext, User, IdentityRole, string>(
            opts =>
            {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.SignIn.RequireConfirmedAccount = false;
                opts.SignIn.RequireConfirmedEmail = false;
            }
        )
            // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityBuilderExtensions.cs#L94
            .AddApiEndpoints() // Adds configuration and services needed to support <see cref="IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi{TUser}(IEndpointRouteBuilder)"/>
            
            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            ;

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
        services.AddTransient<IAuthorizationHandler, Filters.AuthorizationHandler>();
    }

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
            var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
            var factory = x.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(actionContext);
        });
        services.AddScoped<IdentityEmailService<User>>();

        //Microsoft.AspNetCore.Authorization.DefaultAuthorizationService
        services.AddScoped<IEmailSender<User>, IdentityEmailService<User>>();
        services.AddScoped<IEmailSender, EmailSender>();
        // services.AddScoped<IEmailSender, ConsoleEmailSender>();
    }

    private static void RegistertMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToViewModelMappingProfile));
    }
}
