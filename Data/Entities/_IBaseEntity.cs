using Brupper.Core.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brupper.Data.Entities;

public interface IEntityAggregate
{
    string Id { get; }
}

public interface IBaseEntity : IEntityAggregate
{
    IBaseEntity GenerateId();
}

public class EntityAggregate : IBaseEntity
{
    IBaseEntity IBaseEntity.GenerateId() => GenerateId();

    public EntityAggregate GenerateId()
    {
        Id = Guid.NewGuid().ToString();
        return this;
    }

    /// <summary>  </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public virtual string Id { get; set; }
}

[Obsolete("use EntityAggregate instead! [DatabaseGenerated(DatabaseGeneratedOption.Identity)]")]
public class BaseEntity : NotifyPropertyChanged, IBaseEntity
{
    public BaseEntity()
    {
        // okoz performance issue-t? 
        GenerateId();
    }

    public IBaseEntity GenerateId()
    {
        Id = Guid.NewGuid().ToString();
        return this;
    }
    public virtual string Id { get; set; }
}
