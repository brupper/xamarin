using Brupper.Data.Azure.Entities;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Data.Azure.Implementations
{
    public abstract class TableRepository<T> : ITableRepository<T>
        where T : class, IBaseDataObject, new()
    {
        protected static readonly List<T> localCache = new List<T>();
        protected static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        protected ITableStorage Table { get; private set; }

        protected IFileSystem FileSystemService { get; private set; }
        protected IConnectivity ConnectivityService { get; private set; }

        public virtual string Identifier => "Items";

        #region Constructor

        protected TableRepository(ITableStorage table,
            IConnectivity connectivityService,
            IFileSystem fileSystemService)
        {
            Table = table;
            ConnectivityService = connectivityService;
            FileSystemService = fileSystemService;
        }

        #endregion

        public virtual Task<bool> DropTable()
        {
            Table = null;
            return Task.FromResult(true);
        }

        #region IBaseRepository implementation

        public virtual async Task<bool> RemoveItemsAsync(IEnumerable<T> items)
        {
            var list = items?.ToList() ?? new List<T>();

            foreach (var item in list)
            {
                if (localCache.Contains(item))
                {
                    localCache.Remove(item);
                }
            }

            if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                //TODO: await SyncAsync();
                return false;
            }

            bool result = true;
            foreach (var item in list)
            {
                result = result && await RemoveAsync(item);
            }

            return result;
        }

        public async Task InitializeStoreAsync()
        {
            var filePath = Path.Combine(FileSystemService.AppDataDirectory, Identifier);

            try
            {
                await semaphore.WaitAsync(TimeSpan.FromSeconds(5)); //avoid deadlocks...

                if (!File.Exists(filePath))
                {
                    using (File.CreateText(filePath)) ;
                }

                //var json = File.ReadAllText(filePath);
                var json = string.Empty;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    json = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                var entities = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                if (entities.Any())
                {
                    localCache.Clear();
                    localCache.AddRange(entities);
                }
            }
            catch (JsonException)
            {
                // reset file
                File.WriteAllText(filePath, string.Empty);                
            }
            catch (Exception exception)
            {
                //Crashes.TrackError(exception, new Dictionary<string, string> { { "Unable to to InitializeStore :", connectivityService.NetworkAccess.ToString() } });
                Debug.WriteLine(exception);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false)
        {
            await InitializeStoreAsync();
            if (forceRefresh)
            {
                await SyncAsync();
            }

            return localCache.AsEnumerable();
        }

        public virtual async Task<T> GetItemAsync(string id, bool forceRefresh = false)
        {
            await InitializeStoreAsync();
            //TODO await SyncAsync();

            var entity = localCache.FirstOrDefault(x => x.RowKey == id);
            if (entity != null && !forceRefresh)
            {
                return entity;
            }

            if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                return entity;
            }

            var partitionKey = new T().PartitionKey;
            var item = await Table.GetAsync<T>(partitionKey, id);

            if (entity != null)
            {
                localCache.Remove(entity);
            }

            if (item != null)
            {
                localCache.Add(item);
            }

            return item;
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
            await InitializeStoreAsync();

            localCache.Add(item); //Insert into the local store

            if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                //TODO await SyncAsync();
                return false;
            }

            await Table.AddOrUpdateAsync(item); //Insert into the local store
            //TODO await SyncAsync(); //Send changes to the mobile service
            return true;
        }

        public virtual async Task<bool> UpdateAsync(T item)
        {
            // localcache works by reference so no nedd to update entity
            if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                //TODO await SyncAsync();
                return false;
            }

            await InitializeStoreAsync();
            await Table.UpdateAsync(item); //Delete from the local store
            //TODO await SyncAsync(); //Send changes to the mobile service
            return true;
        }

        public virtual async Task<bool> RemoveAsync(T item)
        {
            bool result = false;
            try
            {
                localCache.Remove(item);
                if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
                {
                    //TODO await SyncAsync();
                    return false;
                }

                await InitializeStoreAsync();
                await Table.DeleteAsync(item); //Delete from the local store
                //TODO await SyncAsync(); //Send changes to the mobile service
                result = true;
            }
            catch (Exception exception)
            {
                //Crashes.TrackError(e, new Dictionary<string, string> { { "Unable to remove item :", item.RowKey } });
                Debug.WriteLine(exception);
            }

            return result;
        }

        //Note: do not call SyncAsync with ConfigureAwait(false) because when the authentication token expires,
        //the thread running this method needs to open the Login UI.
        //Also in the method which calls SyncAsync, do not use ConfigureAwait(false) before calling SyncAsync, because once ConfigureAwait(false) is used
        //in the context of an async method, the rest of that method's code may also run on a background thread.
        public virtual async Task<bool> SyncAsync()
        {
            if (ConnectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                // Logger.Instance.Track("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                await semaphore.WaitAsync(TimeSpan.FromSeconds(5)); //avoid deadlocks...

                if (Table == null)
                {
                    //Logger.Instance.Track("Unable to sync items, client is null");
                    return false;
                }

                // TODO: push, pull.
                var reference = new T();
                var remoteItems = await Table.GetAllAsync<T>();
                localCache.Clear();
                localCache.AddRange(remoteItems.Where(x => x.PartitionKey == reference.PartitionKeyInternal));

                var filePath = Path.Combine(FileSystemService.AppDataDirectory, Identifier);
                //using (var fs = File.OpenWrite(filePath))
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    var json = JsonConvert.SerializeObject(localCache);
                    await sw.WriteAsync(json).ConfigureAwait(false);
                }

                //push changes on the sync context before pulling new items
                //await table.SyncContext.PushAsync();
                //await Table.PullAsync($"all{Identifier}", Table.CreateQuery());

            }
            catch (StorageException exception)
            {
                Debug.WriteLine(exception);
            }
            catch (HttpRequestException exception)
            {
                Debug.WriteLine(exception);
            }
            catch (Exception exception)
            {
                //Crashes.TrackError(exception, new Dictionary<string, string> { { "Unable to to push/pull items :", connectivityService.NetworkAccess.ToString() } });
                Debug.WriteLine(exception);
                return false;
            }
            finally
            {
                semaphore.Release();
            }

            return true;
        }

        #endregion
    }

}
