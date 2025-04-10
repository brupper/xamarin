namespace Brupper.AspNetCore.Services.Communication;

public class EmailCommunicationServiceConfig
{
    public string ConnectionString { get; set; } = default!;

    public string FromEmail { get; set; } = default!;

    public string FromName { get; set; } = default!;
    
    public string? ReplyToEmail { get; set; }
}
