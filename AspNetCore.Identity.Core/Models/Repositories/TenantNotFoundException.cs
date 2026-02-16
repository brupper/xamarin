namespace Brupper.AspNetCore.Identity.Repositories;

[Serializable]
public sealed class TenantNotFoundException : Exception
{
    public TenantNotFoundException(string name) : base($"Tenant '{name}' not found") { }
}
