using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Brupper.Data.Azure
{
    /// <summary> . </summary>
    public interface ITableReadonlyRepository<T>
    {
        /// <summary> . </summary>
        string Identifier { get; }

        /// <summary> . </summary>
        Task InitializeStoreAsync();

        /// <summary> . </summary>
        Task<IEnumerable<T>> GetItemsAsync(int skip = 0, int take = 100, bool forceRefresh = false);

        /// <summary> . </summary>
        Task<T> GetItemAsync(string id, bool forceRefresh = false);

        /// <summary> . </summary>
        Task<bool> SyncAsync();
    }

    /// <summary> . </summary>
    public interface ITableRepository<TEntity> : ITableReadonlyRepository<TEntity>//, IRepository<TEntity>
        where TEntity : Data.Entities.IBaseEntity
    {
        /// <summary> . </summary>
        Task<bool> InsertAsync(TEntity item);

        /// <summary> . </summary>
        Task<bool> UpdateAsync(TEntity item);

        /// <summary> . </summary>
        Task<bool> RemoveAsync(TEntity item);

        /// <summary> . </summary>
        Task<bool> RemoveItemsAsync(IEnumerable<TEntity> items);

        /// <summary> . </summary>
        Task<bool> DropTable();
    }
}
