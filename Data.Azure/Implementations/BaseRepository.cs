using Brupper.Data.Azure.Models;
using Microsoft.AppCenter.Crashes;
using MvvmCross;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Data.Azure.Implementations
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : class, IBaseDataObject, new()
    {
        private static readonly List<T> localCache = new List<T>();

        ITableStorage table;
        IFileSystem fileSystemService;
        IConnectivity connectivityService;

        protected ITableStorage Table =>
            table ?? (table = Mvx.IoCProvider.GetSingleton<ITableStorage>());

        protected IFileSystem FileSystemService =>
            fileSystemService ?? (fileSystemService = Mvx.IoCProvider.GetSingleton<IFileSystem>());

        public virtual string Identifier => "Items";

        #region Constructor

        protected BaseRepository(ITableStorage table,
            IConnectivity connectivityService,
            IFileSystem fileSystemService)
        {
            this.table = table;
            this.connectivityService = connectivityService;
            this.fileSystemService = fileSystemService;
        }

        #endregion

        public virtual Task<bool> DropTable()
        {
            table = null;
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

            if (connectivityService.NetworkAccess != NetworkAccess.Internet)
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
            try
            {
                using (var fs = File.OpenRead(Path.Combine(FileSystemService.AppDataDirectory, Identifier)))
                using (var sw = new StreamReader(fs))
                {
                    var json = await sw.ReadToEndAsync();

                    var entities = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                    if (entities.Any())
                    {
                        localCache.Clear();
                        localCache.AddRange(entities);
                    }
                }
            }
            catch (Exception exception)
            {
                Crashes.TrackError(exception, new Dictionary<string, string> { { "Unable to to InitializeStore :", connectivityService.NetworkAccess.ToString() } });
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

            if (connectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                return entity;
            }

            var partitionKey = new T().PartitionKey;
            var item = await Table.GetAsync<T>(id, partitionKey);
            localCache.Add(item);
            return item;
        }

        public virtual async Task<bool> InsertAsync(T item)
        {
            await InitializeStoreAsync();

            localCache.Add(item); //Insert into the local store

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
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
            if (connectivityService.NetworkAccess != NetworkAccess.Internet)
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
                if (connectivityService.NetworkAccess != NetworkAccess.Internet)
                {
                    //TODO await SyncAsync();
                    return false;
                }

                await InitializeStoreAsync();
                await Table.DeleteAsync(item); //Delete from the local store
                //TODO await SyncAsync(); //Send changes to the mobile service
                result = true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string> { { "Unable to remove item :", item.RowKey } });
            }

            return result;
        }

        //Note: do not call SyncAsync with ConfigureAwait(false) because when the authentication token expires,
        //the thread running this method needs to open the Login UI.
        //Also in the method which calls SyncAsync, do not use ConfigureAwait(false) before calling SyncAsync, because once ConfigureAwait(false) is used
        //in the context of an async method, the rest of that method's code may also run on a background thread.
        public virtual async Task<bool> SyncAsync()
        {
            if (connectivityService.NetworkAccess != NetworkAccess.Internet)
            {
                // Logger.Instance.Track("Unable to sync items, we are offline");
                return false;
            }
            try
            {
                if (table == null)
                {
                    //Logger.Instance.Track("Unable to sync items, client is null");
                    return false;
                }

                // TODO: push, pull.
                var remoteItems = await table.GetAllAsync<T>();
                localCache.Clear();
                localCache.AddRange(remoteItems);

                using (var fs = File.OpenWrite(Path.Combine(FileSystemService.AppDataDirectory, Identifier)))
                using (var sw = new StreamWriter(fs))
                {
                    var json = JsonConvert.SerializeObject(localCache);
                    await sw.WriteAsync(json);
                }

                //push changes on the sync context before pulling new items
                //await table.SyncContext.PushAsync();
                //await Table.PullAsync($"all{Identifier}", Table.CreateQuery());
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Unable to to push/pull items :", connectivityService.NetworkAccess.ToString() } });
                return false;
            }
            return true;
        }

        #endregion
    }

}
