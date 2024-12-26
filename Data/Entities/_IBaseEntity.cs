using Brupper.Core.Models;
using System;

namespace Brupper.Data.Entities;

public interface IEntity
{
    string Id { get; }
}

public interface IBaseEntity : IEntity
{
    IBaseEntity GenerateId();
}

public class BaseEntity : NotifyPropertyChanged, IBaseEntity
{
    public BaseEntity()
    {
        GenerateId();
    }

    public IBaseEntity GenerateId()
    {
        Id = Guid.NewGuid().ToString();
        return this;
    }

    public virtual string Id { get; set; }
}
