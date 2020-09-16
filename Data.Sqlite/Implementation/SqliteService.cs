using Brupper.Data.Sqlite.Models.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Brupper.Data.Sqlite.Implementation
{
    public class SqliteService : ISqliteService
    {
        private SQLiteAsyncConnection sqliteAsyncConnection;

        private static readonly AsyncLock asyncLock = new AsyncLock();
        private static readonly bool storeDateTimeAsTicks = false;
        private string databasePath;

        public bool IsEncrypted { get; private set; }

        public bool IsInitialized
            => sqliteAsyncConnection != null;

        #region Contructors

        public SqliteService()
        {

        }

        #endregion

        #region ISqliteService

        public void InitConnection(string localFile, string encryptionKey = null)
        {
            CloseConnection();
            databasePath = localFile;

            try
            {
                try
                {
                    sqliteAsyncConnection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks, encryptionKey);
                }
                catch (SQLiteException)
                {
                    // Unable to create encrypted database.
                }

                if (sqliteAsyncConnection == null)
                {
                    sqliteAsyncConnection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            IsEncrypted = !string.IsNullOrEmpty(encryptionKey);
        }


        public void CloseConnection()
        {
            SQLiteAsyncConnection.ResetPool();
            sqliteAsyncConnection = null;
        }

        public async Task<SqlDbVersion> GetVersionAsync()
        {
            using (await asyncLock.LockAsync())
            {
                SqlDbVersion result = SqlDbVersion.None;

                if (await TableExists<SqlDbVersion>())
                {
                    result = await sqliteAsyncConnection.Table<SqlDbVersion>().FirstOrDefaultAsync() ?? SqlDbVersion.None;
                }

                return result;
            }
        }

        public async Task<int> UpdateAsync<T>(T objectToUpdate) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.UpdateAsync(objectToUpdate);
            }
        }

        public async Task<List<T>> GetAllItemsAsync<T>() where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                AsyncTableQuery<T> query = sqliteAsyncConnection.Table<T>();

                return await query.ToListAsync();
            }
        }

        public async Task ResetContentAsync<T>(IEnumerable<T> newValues) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                foreach (var item in newValues)
                {
                    await sqliteAsyncConnection.InsertOrReplaceAsync(item);
                }
            }
        }

        public async Task RemoveItemAsync<T>(object itemId) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                await sqliteAsyncConnection.RunInTransactionAsync(c =>
                {
                    var mapping = c.GetMapping<T>();
                    c.Execute("DELETE FROM " + mapping.TableName + " WHERE  Id = '" + itemId.ToString() + "'");
                });
            }
        }

        public async Task RemoveItemByUuidAsync<T>(object itemUuid) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                await sqliteAsyncConnection.RunInTransactionAsync(c =>
                {
                    var mapping = c.GetMapping<T>();
                    c.Execute("DELETE FROM " + mapping.TableName + " WHERE  Uuid = '" + itemUuid.ToString() + "'");
                });
            }
        }

        public async Task RemoveAllItems<T>() where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                await RemoveAllItemsInternal<T>();
            }
        }

        public async Task ExecuteScriptAsync(IEnumerable<string> script)
        {
            using (await asyncLock.LockAsync())
            {
                await sqliteAsyncConnection.RunInTransactionAsync(tran =>
                {
                    foreach (var query in script)
                    {
                        tran.Execute(query);
                    }
                });
            }
        }

        public async Task InsertMultipleAsync<T>(IEnumerable<T> items)
        {
            using (await asyncLock.LockAsync())
            {
                await sqliteAsyncConnection.InsertAllAsync(items);
            }
        }

        public async Task<int> ExecuteAsync(string query, params object[] args)
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.ExecuteAsync(query, args);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, params object[] args)
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.ExecuteScalarAsync<T>(sql, args);
            }
        }

        public async Task<int> InsertAsync(object item)
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.InsertAsync(item);
            }
        }

        public async Task<int> InsertOrReplaceAsync(object item)
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.InsertOrReplaceAsync(item);
            }
        }

        public async Task<int> DeleteAsync(object item)
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.DeleteAsync(item);
            }
        }

        public async Task<int> DropTableAsync<T>() where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.DropTableAsync<T>();
            }
        }

        public async Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.FindAsync(predicate);
            }
        }

        public async Task<List<T>> QueryAsync<T>(string sql, params object[] args) where T : new()
        {
            using (await asyncLock.LockAsync())
            {
                return await sqliteAsyncConnection.QueryAsync<T>(sql, args);
            }
        }

        #endregion

        private async Task<bool> TableExists<T>() where T : new()
        {
            var tableCount = await sqliteAsyncConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND tbl_name = ?", typeof(T).Name);

            return tableCount == 1;
        }

        private Task RemoveAllItemsInternal<T>() where T : new()
        {
            // Use transaction to get the underlying SQLiteConnection.
            // We cannot use DeleteAsync because we need to have the property PrimaryKey
            // which is platform dependent.
            return sqliteAsyncConnection.RunInTransactionAsync(c =>
            {
                var mapping = c.GetMapping<T>();
                c.Execute("DELETE FROM " + mapping.TableName);
            });
        }

    }
}
