namespace Brupper.AspNetCore;

public interface IUserContextAccessor
{
    string Name { get; }
    string Email { get; }
    string Id { get; }
    string? TenantId { get; }
    bool IsNotSuperAdmin { get; }
}

public class AnonymUserContextAccessor : IUserContextAccessor
{
    public string Name => "ANONYMUS";
    public string Email => "ANONYMUS";
    public string Id => Guid.Empty.ToString();
    public string? TenantId => null;
    public bool IsNotSuperAdmin => false;
}
