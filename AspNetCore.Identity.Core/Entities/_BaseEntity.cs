using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Diagnostics;

namespace Brupper.AspNetCore.Identity.Entities;

public class BaseEntity : Brupper.Data.Entities.EntityAggregate
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

    public BaseEntity() : base() { PartitionKey = "identity"; }

    #endregion

}
