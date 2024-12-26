namespace Brupper.Data.Entities;

/// <summary>
/// Used to standardize soft deleting entities.
/// Soft-delete entities are not actually deleted,
/// marked as Deleted = true in the database,
/// but can not be retrieved to the application.
/// </summary>
public interface ISoftDelete
{
    /// <summary> Used to mark an Entity as 'Deleted'. </summary>
    bool Deleted { get; set; }
}
