using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data.Azure.Implementations
{
    public class AzureTableStorage : ITableStorage
    {
        protected readonly IConfiguration configuration;
        protected static readonly ConcurrentDictionary<string, TableClient> tables = new();
        protected static readonly ConcurrentDictionary<Type, string> boundTypeInfos = new();

        public AzureTableStorage(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<T> GetAsync<T>(string partitionKey, string rowKey)
            where T : class, ITableEntity
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.GetEntityAsync<T>(partitionKey, rowKey).ConfigureAwait(false);

            return result.Value as T;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var entities = new List<T>();

            // TODO: filter by partitionkey
            var queryResult = table.QueryAsync<T>(x => true);

            var enumerator = queryResult.GetAsyncEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    entities.Add(enumerator.Current);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }


            return entities;
        }

        // /*
        public async Task<IEnumerable<T>> QueryAsync<T>(
            Expression<Func<T, bool>> filter,
            int? maxPerPage = null,
            IEnumerable<string> select = null,
            CancellationToken cancellationToken = default) where T : class, ITableEntity
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var entities = new List<T>();

            // TODO: filter by partitionkey
            var queryResult = table.QueryAsync(filter, maxPerPage, select, cancellationToken);

            var enumerator = queryResult.GetAsyncEnumerator();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    entities.Add(enumerator.Current);
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }


            return entities;


            //bool shouldConsiderTakeCount = query.TakeCount.HasValue;

            //return shouldConsiderTakeCount ?
            //    await QueryAsyncWithTakeCount(table, query).ConfigureAwait(false) :
            //    await QueryAsync(table, query).ConfigureAwait(false);
        }
        // */

        public async Task<T> AddOrUpdateAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.UpsertEntityAsync(entity).ConfigureAwait(false);

            return result.ContentStream as T;
        }

        public async Task<T> DeleteAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.DeleteEntityAsync(entity.PartitionKey, entity.RowKey).ConfigureAwait(false);

            return result.Content as T;
        }

        public async Task<T> AddAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.AddEntityAsync(entity).ConfigureAwait(false);

            return result.Content as T;
        }

        /*
        // TODO: https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/tables/Azure.Data.Tables/samples/Sample6TransactionalBatch.md
        public async Task<IEnumerable<T>> AddBatchAsync<T>(IEnumerable<ITableEntity> entities, BatchOperationOptions options = null)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            options = options ?? new BatchOperationOptions();

            var tasks = new List<TableTransactionAction>();

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

            // Submit the batch.
            Response<IReadOnlyList<Response>> response = await client.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results.SelectMany(tableResults => tableResults, (tr, r) => r.Result as T);
        }

        public async Task<IEnumerable<T>> DeleteBatchAsync<T>(IEnumerable<ITableEntity> entities)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var tasks = new List<Task<IList<TableResult>>>();

            const int addBatchOperationLimit = 100;
            var entitiesOffset = 0;
            var tableEntities = entities?.ToList() ?? new List<ITableEntity>();
            while (entitiesOffset < tableEntities.Count)
            {
                var entitiesToAdd = tableEntities.Skip(entitiesOffset).Take(addBatchOperationLimit).ToList();
                entitiesOffset += entitiesToAdd.Count;

                Action<TableBatchOperation, ITableEntity> batchInsertOperation = (bo, entity) => bo.Delete(entity);

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
        // */

        public async Task<T> UpdateAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace).ConfigureAwait(false);

            return result.Content as T;
        }

        public async Task<T> UpdatePartialAsync<T>(T entity)
            where T : class, ITableEntity, new()
        {
            var tableName = GetTableName<T>();
            var table = await EnsureTableAsync(tableName).ConfigureAwait(false);

            var result = await table.UpdateEntityAsync(entity, new ETag("*"), TableUpdateMode.Merge).ConfigureAwait(false);

            return result.Content as T;
        }

        protected async Task<TableClient> EnsureTableAsync(string tableName)
        {
            if (!tables.ContainsKey(tableName))
            {
                var table = new TableClient(configuration.AzureTableConnectionString, tableName);

                await table.CreateIfNotExistsAsync().ConfigureAwait(false);

                tables[tableName] = table;
            }

            return tables[tableName];
        }

        /*
        protected async Task<IEnumerable<T>> QueryAsync<T>(TableClient table, TableQuery<T> query)
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

        protected async Task<IEnumerable<T>> QueryAsyncWithTakeCount<T>(TableClient table, TableQuery<T> query)
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
        // */

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
