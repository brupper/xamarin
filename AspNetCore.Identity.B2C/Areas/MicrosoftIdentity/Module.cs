using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Net;

namespace Brupper.AspNetCore.Identity.B2C.Areas.MicrosoftIdentity;

public static class Module
{
    public static void SetupMicrosoftIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // global::Microsoft.Identity.Web.MicrosoftIdentityOptions vs GraphApiKeyServiceClientCredentials

        var initialScopes = configuration["MicrosoftGraph:Scopes"]?.Split(' ') ?? new string[0];

        // to get JSON config from: https://bab2c.b2clogin.com/tfp/bab2c.onmicrosoft.com/B2C_1_InspEx_1/v2.0/.well-known/openid-configuration
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                               | SecurityProtocolType.Tls11
                                               | SecurityProtocolType.Tls12
                                               //| SecurityProtocolType.Ssl3
                                               ;


        var scheme = $"{OpenIdConnectDefaults.AuthenticationScheme}";
        var cookieScheme = IdentityConstants.ExternalScheme; // ... AuthenticateAsync(IdentityConstants.ExternalScheme);

        /*
        ez mukodik:
        services
            .AddAuthentication()
            // already exists .AddCookie(cookieScheme)
            // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins
            .AddMicrosoftAccount(microsoftOptions => configuration.Bind("AzureAdB2C", microsoftOptions));
*/
        services.AddAuthentication()
                        .AddOpenIdConnect(async options => //"b2c", "Azure B2C",
                        {
                            options.Authority = $"{configuration["AzureAdB2C:Instance"]}/{configuration["AzureAdB2C:Tenant"]}/{configuration["AzureAdB2C:SignUpSignInPolicyId"]}/v2.0/";
                            //https://bab2c.b2clogin.com/tfp/bab2c.onmicrosoft.com/B2C_1_InspEx_1/v2.0/.well-known/openid-configuration
                            configuration.Bind("AzureAdB2C", options);

                            options.ClaimActions.MapUniqueJsonKey(Microsoft.Identity.Web.ClaimConstants.ObjectId, Microsoft.Identity.Web.ClaimConstants.ObjectId);
                            options.GetClaimsFromUserInfoEndpoint = true;

                            //options.ClientId = configuration["AzureAdB2C:ClientId"];
                            //options.ClientSecret = configuration["AzureAdB2C:ClientSecret"];
                            //options.CallbackPath = configuration["AzureAdB2C:CallbackPath"];
                            //options.SignInScheme = IdentityConstants.ExternalScheme;
                            //options.DisableTelemetry = true;
                            //options.GetClaimsFromUserInfoEndpoint = true;

                            //options.ResponseType = "code id_token";                
                            //options.UsePkce = false; // apple does not currently support PKCE (April 2021)
                            options.Events.OnAuthorizationCodeReceived = context => Task.CompletedTask;
                            options.Events.OnRedirectToIdentityProvider = context => Task.FromResult(0);
                            options.Events.OnAuthenticationFailed = context => Task.CompletedTask;
                            options.Events.OnTokenValidated = async context =>
                            {
                                /*
                                // Get user's immutable object id from claims that came from Azure AD
                                string oid = context.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

                                var db = context.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();

                                //Check is user a super admin
                                //var isSuperAdmin = await db.SuperAdmins.AnyAsync(a => a.ObjectId == oid);
                                var userId = db.GetUserId(context.Principal);
                                var isSuperAdmin = await db.GetRolesAsync(new IdentityUser { Id = oid });
                                if (isSuperAdmin.Any()) //Add claim if they are
                                {

                                    context.Principal.AddIdentity(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Role, "superadmin") }));
                                }
                                // */
                            };
                        })
       ; 
        // */


        services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options => configuration.Bind("AzureAdB2C", options))
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddMicrosoftGraph(configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches()
            ;

        //services.AddMicrosoftGraph(opt => configuration.Bind("DownstreamApi", opt));

        services.AddTransient(p => configuration.GetSection("AzureAdB2C").Get<GraphApiKeyServiceClientCredentials>());
        services.AddSingleton<IUserGraphManager, UserGraphManager>();

        services.AddScoped<IAuthorizationHandler, Filters.AuthorizationHandler>();

        services.AddRazorPages().AddMicrosoftIdentityUI();

#if DEBUG
        // This should be removed in production and the authority url changed to https
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
#endif
    }
}
