using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Data.Azure
{
    public interface ITableReadonlyRepository<T>
    {
        string Identifier { get; }
        Task InitializeStoreAsync();
        Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false);
        Task<T> GetItemAsync(string id, bool forceRefresh = false);
        Task<bool> SyncAsync();
    }

    public interface ITableRepository<TEntity> : ITableReadonlyRepository<TEntity>//, IRepository<TEntity>
        where TEntity : Data.Entities.IBaseEntity
    {
        Task<bool> InsertAsync(TEntity item);
        Task<bool> UpdateAsync(TEntity item);
        Task<bool> RemoveAsync(TEntity item);
        Task<bool> RemoveItemsAsync(IEnumerable<TEntity> items);
        Task<bool> DropTable();
    }
}
