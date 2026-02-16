using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Brupper.AspNetCore.Identity.IdentityHostingStartup))]

namespace Brupper.AspNetCore.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => { });
    }
}
