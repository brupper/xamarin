using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Brupper.AspNetCore.Identity.B2C.IdentityHostingStartup))]

namespace Brupper.AspNetCore.Identity.B2C;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) => { });
    }
}
