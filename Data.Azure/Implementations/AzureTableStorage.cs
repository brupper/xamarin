using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brupper.Data.Azure.Implementations
{
    public class AzureTableStorage : ITableStorage
    {
        protected readonly CloudTableClient client;
        protected static readonly ConcurrentDictionary<string, CloudTable> tables = new ConcurrentDictionary<string, CloudTable>();
        protected static readonly ConcurrentDictionary<Type, string> boundTypeInfos = new ConcurrentDictionary<Type, string>();

        public AzureTableStorage(IConfiguration configuration)
        {
            var account = CloudStorageAccount.Parse(configuration.AzureTableConnectionString);
            client = account.CreateCloudTableClient();
        }

        public async Task<T> GetAsync<T>(string partitionKey, string rowKey)
            where T : class, ITableEntity
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            TableResult result = await table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            return result.Result as T;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableContinuationToken token = null;
            var entities = new List<T>();
            do
            {
                // TODO: filter by partitionkey
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery<T>(), token).ConfigureAwait(false);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(TableQuery<T> query)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            bool shouldConsiderTakeCount = query.TakeCount.HasValue;

            return shouldConsiderTakeCount ?
                await QueryAsyncWithTakeCount(table, query).ConfigureAwait(false) :
                await QueryAsync(table, query).ConfigureAwait(false);
        }

        public async Task<T> AddOrUpdateAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(entity);

            TableResult result = await table.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);

            return result.Result as T;
        }

        public async Task<T> DeleteAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableOperation deleteOperation = TableOperation.Delete(entity);

            TableResult result = await table.ExecuteAsync(deleteOperation).ConfigureAwait(false);

            return result.Result as T;
        }

        public async Task<T> AddAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableOperation insertOperation = TableOperation.Insert(entity);

            TableResult result = await table.ExecuteAsync(insertOperation).ConfigureAwait(false);

            return result.Result as T;
        }

        public async Task<IEnumerable<T>> AddBatchAsync<T>(IEnumerable<ITableEntity> entities, BatchOperationOptions options = null)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            options = options ?? new BatchOperationOptions();

            var tasks = new List<Task<IList<TableResult>>>();

            const int addBatchOperationLimit = 100;
            var entitiesOffset = 0;
            var tableEntities = entities?.ToList() ?? new List<ITableEntity>();
            while (entitiesOffset < tableEntities.Count)
            {
                var entitiesToAdd = tableEntities.Skip(entitiesOffset).Take(addBatchOperationLimit).ToList();
                entitiesOffset += entitiesToAdd.Count;

                Action<TableBatchOperation, ITableEntity> batchInsertOperation = null;
                switch (options.BatchInsertMethod)
                {
                    case BatchInsertMethod.Insert:
                        batchInsertOperation = (bo, entity) => bo.Insert(entity);
                        break;
                    case BatchInsertMethod.InsertOrReplace:
                        batchInsertOperation = (bo, entity) => bo.InsertOrReplace(entity);
                        break;
                    case BatchInsertMethod.InsertOrMerge:
                        batchInsertOperation = (bo, entity) => bo.InsertOrMerge(entity);
                        break;
                }

                var batchOperation = new TableBatchOperation();
                foreach (var entity in entitiesToAdd)
                {
                    batchInsertOperation?.Invoke(batchOperation, entity);
                }
                tasks.Add(table.ExecuteBatchAsync(batchOperation));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results.SelectMany(tableResults => tableResults, (tr, r) => r.Result as T);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            TableOperation replaceOperation = TableOperation.Replace(entity);

            TableResult result = await table.ExecuteAsync(replaceOperation).ConfigureAwait(false);

            return result.Result as T;
        }


        public async Task<T> UpdatePartialAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            entity.ETag = "*";  // important!
            var replaceOperation = TableOperation.Merge(entity);

            var result = await table.ExecuteAsync(replaceOperation).ConfigureAwait(false);

            return result.Result as T;
        }

        protected async Task<CloudTable> EnsureTableAsync(string tableName)
        {
            if (!tables.ContainsKey(tableName))
            {
                var table = client.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync().ConfigureAwait(false);
                tables[tableName] = table;
            }

            return tables[tableName];
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(CloudTable table, TableQuery<T> query)
            where T : class, ITableEntity, new()
        {
            var entities = new List<T>();

            TableContinuationToken token = null;
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null);

            return entities;
        }

        protected async Task<IEnumerable<T>> QueryAsyncWithTakeCount<T>(CloudTable table, TableQuery<T> query)
            where T : class, ITableEntity, new()
        {
            var entities = new List<T>();

            const int maxEntitiesPerQueryLimit = 1000;
            var totalTakeCount = query.TakeCount;
            var remainingRecordsToTake = query.TakeCount;

            TableContinuationToken token = null;
            do
            {
                query.TakeCount = remainingRecordsToTake >= maxEntitiesPerQueryLimit ? maxEntitiesPerQueryLimit : remainingRecordsToTake;
                remainingRecordsToTake -= query.TakeCount;

                var queryResult = await table.ExecuteQuerySegmentedAsync(query, token).ConfigureAwait(false);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (entities.Count < totalTakeCount && token != null);

            return entities;
        }

        protected static string GetTableName<T>()
        {
            var type = typeof(T);
            if (!boundTypeInfos.TryGetValue(typeof(T), out string tableName))
            {
                boundTypeInfos[type] = type.GetAttributeValue<TableNameAttribute, string>(tn => tn.Name) ?? nameof(T);

                return boundTypeInfos[type];
            }

            return tableName;
        }
    }
}
