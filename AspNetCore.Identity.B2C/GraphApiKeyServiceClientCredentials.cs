namespace Brupper.AspNetCore.Identity.B2C;

public class GraphApiKeyServiceClientCredentials
{
    public string Tenant { get; set; } = default!;

    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;
}
