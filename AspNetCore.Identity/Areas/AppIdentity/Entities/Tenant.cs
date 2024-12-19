namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = default!;

    public string Contact { get; set; } = default!;

    public virtual string Phone { get; set; } = default!;

    public virtual string Email { get; set; } = default!;

    /// <summary> Inactive </summary>
    public virtual bool Deleted { get; set; } = default!;

    #region IAddress

    /// <inheritdoc/>
    public virtual string? Zip { get; set; } = default!;

    /// <inheritdoc/>
    public virtual string? City { get; set; } = default!;

    /// <inheritdoc/>
    public virtual string? Address { get; set; } = default!;

    /// <inheritdoc/>
    public virtual string? Number { get; set; } = default!;

    /// <inheritdoc/>
    public virtual string? PostalZip { get; set; }

    /// <inheritdoc/>
    public virtual string? PostalCity { get; set; }

    /// <inheritdoc/>
    public virtual string? PostalAddress { get; set; }

    /// <inheritdoc/>
    public virtual string? PostalNumber { get; set; }

    #endregion

    #region Navigation Properties

    public List<ModuleReference> Licences { get; set; } = new();

    #endregion
}
