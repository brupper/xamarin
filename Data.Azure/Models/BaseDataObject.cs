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
}
