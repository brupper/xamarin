using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Data.Azure
{
    public interface ITableStorage
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class, ITableEntity, new();

        /// <summary>
        /// Gets entities by query. 
        /// Supports TakeCount parameter.
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(TableQuery<T> query) where T : class, ITableEntity, new();

        Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity;

        /// <summary> Adds the or update entity. </summary>
        Task<T> AddOrUpdateAsync<T>(T entity) where T : class, ITableEntity, new();

        Task<T> DeleteAsync<T>(T entity) where T : class, ITableEntity, new();

        Task<T> AddAsync<T>(T entity) where T : class, ITableEntity, new();

        /// <summary> Insert a batch of entities. Support adding more than 100 entities. </summary>
        Task<IEnumerable<T>> AddBatchAsync<T>(IEnumerable<ITableEntity> entities, BatchOperationOptions options) where T : class, ITableEntity, new();

        Task<T> UpdateAsync<T>(T entity) where T : class, ITableEntity, new();

        Task<T> UpdatePartialAsync<T>(T entity) where T : class, ITableEntity, new();
    }

    public class BatchOperationOptions
    {
        public BatchInsertMethod BatchInsertMethod { get; set; }
    }

    public enum BatchInsertMethod
    {
        Insert,
        InsertOrReplace,
        InsertOrMerge
    }
}
