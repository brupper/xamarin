using Brupper.Data.Sqlite.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Brupper.Data.Sqlite
{
    public interface ISqliteService
    {
        bool IsEncrypted { get; }

        bool IsInitialized { get; }

        Task<SqlDbVersion> GetVersionAsync();

        Task<int> UpdateAsync<T>(T objectToUpdate) where T : new();

        Task<List<T>> GetAllItemsAsync<T>() where T : new();

        Task RemoveAllItems<T>() where T : new();

        Task ResetContentAsync<T>(IEnumerable<T> newValues) where T : new();

        Task RemoveItemAsync<T>(object itemId) where T : new();

        Task RemoveItemByUuidAsync<T>(object itemId) where T : new();

        Task<int> ExecuteAsync(string query, params object[] args);

        Task<T> ExecuteScalarAsync<T>(string sql, params object[] args);

        Task<int> InsertAsync(object item);

        Task<int> InsertOrReplaceAsync(object item);

        Task<int> DeleteAsync(object item);

        Task<int> DropTableAsync<T>() where T : new();

        Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<List<T>> QueryAsync<T>(string sql, params object[] args) where T : new();

        Task ExecuteScriptAsync(IEnumerable<string> script);

        void InitConnection(string localFile, string encryptionKey = null);

        void CloseConnection();

        Task InsertMultipleAsync<T>(IEnumerable<T> items);
    }
}
