using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace Brupper.Data.Azure.Models
{
    public interface IBaseDataObject : ITableEntity
    {
        string PartitionKeyInternal { get; }
    }

    public abstract class BaseDataObject : TableEntity, IBaseDataObject
    {
        [JsonIgnore]
        public abstract string PartitionKeyInternal { get; }

        protected BaseDataObject()
        {
            PartitionKey = PartitionKeyInternal;
            RowKey = Guid.NewGuid().ToString();
        }
    }

    public abstract class BaseDataObject<T> : TableEntityAdapter<T>, IBaseDataObject
    {
        [JsonIgnore]
        public abstract string PartitionKeyInternal { get; }

        protected BaseDataObject()
        {
            PartitionKey = PartitionKeyInternal;
            RowKey = Guid.NewGuid().ToString();
        }

        public BaseDataObject(T originalEntity) : base(originalEntity)
        {
            PartitionKey = PartitionKeyInternal;
            RowKey = Guid.NewGuid().ToString();
        }

        public BaseDataObject(T originalEntity, string partitionKey, string rowKey) : base(originalEntity, partitionKey, rowKey)
        {
            PartitionKey = PartitionKeyInternal;
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
