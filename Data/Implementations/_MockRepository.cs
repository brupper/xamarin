using Brupper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data
{
    public class MockRepository<TEntity> : IRepository<TEntity>
        where TEntity : IBaseEntity
    {
        private bool disposed;

        internal static readonly List<TEntity> dbSet = new List<TEntity>();

        #region Constructor

        static MockRepository() { }

        public MockRepository() { }

        #endregion

        #region IRepository<TEntity> implementation

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            IQueryable<TEntity> query = dbSet.AsQueryable();

            cancellationToken.ThrowIfCancellationRequested();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            if (id == null)
            {
                TEntity nullResult = default;
                return Task.FromResult<TEntity>(nullResult);
            }

#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            return Task.FromResult<TEntity>(dbSet.AsQueryable().FirstOrDefault(x => x.Id == id));
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            dbSet.Add(entity);
            await SaveAsync();
        }

        public Task InsertOrUpdateAsync(TEntity entity)
        {
            if (!dbSet.Contains(entity))
                dbSet.Add(entity);
            return SaveAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            TEntity entityToDelete = dbSet.FirstOrDefault(x => x.Id == id);
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
            await DeleteAsync(entityToDelete);
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {
            dbSet.Remove(entityToDelete);

            await SaveAsync();
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            await SaveAsync();
        }

        public virtual Task SaveAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // ...
                }

                disposed = true;
            }
        }

        ~MockRepository()
        {
            Dispose(false);
        }

        #endregion
    }
}
