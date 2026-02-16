namespace Brupper.Data.Entities;

public interface IHasOwner
{
    /// <summary> ID of user. Indicates that who owns the record. It can differ from CreatedBy attribute. </summary>
    string? OwnerId { get; set; }
}
