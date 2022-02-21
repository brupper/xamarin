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
    /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public virtual Task<TEntity> GetByIdAsync(object id)
        {
            if (id == null)
            {
                return Task.FromResult<TEntity>(null);
            }

            return dbSet.FindAsync(id).AsTask();
        }

        /// <inheritdoc/>
        public virtual async Task InsertAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        /// <inheritdoc/>
        public virtual async Task InsertOrUpdateAsync(TEntity entity)
        {
            var oldEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            if (oldEntity == null)
            {
                await InsertAsync(entity);
            }
            else
            {
                await UpdateAsync(entity);
            }
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            // {"The instance of entity type 'TEntity' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values."}
            var x = context.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => x.Entity?.Id == entity.Id);
            if (x?.Entity != null)
            {
                context.Entry(x.Entity).State = EntityState.Detached;
            }

            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);

            await SaveAsync();
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            // {"The instance of entity type 'TEntity' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values."}
            var x = context.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => x.Entity?.Id == entityToUpdate.Id);
            if (x?.Entity != null)
            {
                context.Entry(x.Entity).State = EntityState.Detached;
            }

            dbSet.Attach(entityToUpdate);
            context.SetModified(entityToUpdate);

            await SaveAsync();
        }

        /// <inheritdoc/>
        public virtual Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        #endregion

        #region IDisposable implementation

        /// <inheritdoc/>
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
