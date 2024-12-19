using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

//[assembly: UIFramework("Bootstrap5")] // https://github.com/dotnet/aspnetcore/blob/1dcf7acfacf0fe154adcc23270cb0da11ff44ace/src/Identity/UI/src/UIFramework.cs

namespace Brupper.AspNetCore.Identity.Areas.Identity;

public static class Module
{
    public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // to get JSON config from: https://bab2c.b2clogin.com/tfp/bab2c.onmicrosoft.com/B2C_1_InspEx_1/v2.0/.well-known/openid-configuration
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                               | SecurityProtocolType.Tls11
                                               | SecurityProtocolType.Tls12
                                               //| SecurityProtocolType.Ssl3
                                               ;
    }
}
