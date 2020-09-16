using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Brupper.Data.Azure.Models
{
    public interface IBaseDataObject : ITableEntity
    {
    }

    public abstract class BaseDataObject : TableEntity, IBaseDataObject
    {
        protected abstract string PartitionKeyInternal { get; }

        protected BaseDataObject()
        {
            PartitionKey = PartitionKeyInternal;
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
