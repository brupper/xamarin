using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;

/// <summary> ID property is typically the foreign key as well. </summary>
[Owned]
public class BaseOwnedEntity
{
    #region Constructors

    public BaseOwnedEntity(string primaryKey) : base()
    {
        Guard.IsNotNullOrWhiteSpace(primaryKey);

        Id = primaryKey;
    }

    public BaseOwnedEntity() : this(Guid.NewGuid().ToString()) { } // generates Id a.k.a. PrimaryKey

    #endregion

    /// <summary> Typically foreign key </summary>
    public virtual string Id { get; set; } = default!;
}
