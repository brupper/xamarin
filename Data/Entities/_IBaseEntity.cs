using Brupper.Core.Models;
using System;

namespace Brupper.Data.Entities
{
    public interface IBaseEntity
    {
        string Id { get; }

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
}
