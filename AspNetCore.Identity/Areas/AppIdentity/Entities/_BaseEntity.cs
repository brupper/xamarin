using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Diagnostics;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;

public class BaseEntity : Brupper.Data.Entities.BaseEntity
{
    [Required]
    public virtual string PartitionKey { get; set; } = default!;

    #region Constructors

    public BaseEntity(string primaryKey, string partitionKey)
    {
        Guard.IsNotNullOrWhiteSpace(primaryKey);
        Guard.IsNotNullOrWhiteSpace(partitionKey);

        Id = primaryKey;
        PartitionKey = partitionKey;
    }

    public BaseEntity() : base() { PartitionKey = "identity"; } // generates Id a.k.a. PrimaryKey

    #endregion

}
