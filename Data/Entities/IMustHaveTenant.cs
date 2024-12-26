namespace Brupper.Data.Entities;

/// <summary> Implement this interface for an entity which must have TenantId. </summary>
public interface IMustHaveTenant
{
    /// <summary> TenantId of this entity. </summary>
    string TenantId { get; set; }
}