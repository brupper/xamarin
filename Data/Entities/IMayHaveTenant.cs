namespace Brupper.Data.Entities;

/// <summary> Implement this interface for an entity which may optionally have TenantId. </summary>
public interface IMayHaveTenant
{
    /// <summary> TenantId of this entity. </summary>
    string? TenantId { get; set; }
}