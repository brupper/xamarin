using Brupper.Data.EF.Contexts;
using Brupper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data.EF
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : BaseEntity
    {
        private bool disposed;

        protected ADataContext context;
        protected DbSet<TEntity> dbSet;

        #region Constructor

        public Repository(ADataContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        #endregion

        #region IRepository<TEntity> implementation

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();

            cancellationToken.ThrowIfCancellationRequested();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.SafeInclude(includeProperties);

            cancellationToken.ThrowIfCancellationRequested();
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync(cancellationToken);
            }
            else
            {
                return await query.ToListAsync(cancellationToken);
            }
        }

        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            if (id == null)
            {
                return Task.FromResult<TEntity>(null);
            }

            return dbSet.FindAsync(id).AsTask();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);

            await SaveAsync();
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.SetModified(entityToUpdate);

            await SaveAsync();
        }

        public virtual Task SaveAsync()
        {
            return context.SaveChangesAsync();
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
                    context?.Dispose();
                    context = null;
                    dbSet = null;
                }

                disposed = true;
            }
        }

        ~Repository()
        {
            Dispose(false);
        }

        #endregion
    }

}
