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
        where TEntity : class, Entities.IBaseEntity, new()
    {
        private bool disposed;
        protected bool tracking = true;

        protected DbContext context;
        protected DbSet<TEntity> dbSet;

        #region Constructor

        public Repository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();
        }

        #endregion

        #region IRepository<TEntity> implementation

        ///// <inheritdoc/>
        //public IRepository<TEntity> WithTracking()
        //{
        //    tracking = true;
        //    return this;
        //}

        ///// <inheritdoc/>
        //public IRepository<TEntity> WithoutTracking()
        //{
        //    tracking = false;
        //    return this;
        //}

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageSize = null,
            int pageNumber = 0,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = tracking ? (IQueryable<TEntity>)dbSet : dbSet.AsNoTracking();

            cancellationToken.ThrowIfCancellationRequested();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.SafeInclude(includeProperties);

            cancellationToken.ThrowIfCancellationRequested();
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (pageSize.HasValue)
            {
                query = query.Skip(pageNumber * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync(cancellationToken);

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
            if (entity.Id == null) { entity.GenerateId(); }

            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        /// <inheritdoc/>
        public virtual async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            context.AddRange(entities);
            await SaveAsync();
        }

        /// <inheritdoc/>
        public virtual async Task InsertOrUpdateAsync(TEntity entity)
        {
            var oldEntity = context.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => x.Entity?.Id == entity.Id)?.Entity;
            if (oldEntity == null)
            {
                oldEntity = await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
            }

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
        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            if (entityToUpdate?.Id == null)
            {
                return;
            }

            // {"The instance of entity type 'TEntity' cannot be tracked 
            // because another instance with the same key value for {'Id'}
            // is already being tracked. When attaching existing entities,
            // ensure that only one entity instance with a given key value
            // is attached. Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the conflicting key values."}
            //context.DetachLocal(entityToUpdate);

            // pedig ez lenne a helyes eljaras ... 
            // https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
            dbSet.Attach(entityToUpdate);        // method call utan kezdi el trackelni az record-ot...
            context.SetModified(entityToUpdate); // update statement that will update all the fields of the entity.

            // ez inspex mobil oldali pelda:
            //var current = await GetByIdAsync(entityToUpdate.Id);
            //context.Entry(current).CurrentValues.SetValues(entityToUpdate);

            await SaveAsync();
        }


        /// <inheritdoc/>
        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            context.RemoveRange(entities);
            await SaveAsync();
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
        public virtual Task SaveAsync()
        {
            return context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public Task Revert(TEntity entity)
        {
            context.Entry(entity).Reload();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task RevertAsync(TEntity entity)
        {
            return context.Entry(entity).ReloadAsync();
        }

        #endregion

        #region Helpers

        public virtual async Task<TEntity> FirstOrDefault(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "",
           CancellationToken cancellationToken = default)
        {
            return (await GetAsync(filter, orderBy, 1, 0, includeProperties, cancellationToken)).FirstOrDefault();
        }

        public virtual async Task<int> Count(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = dbSet.AsNoTracking();
            cancellationToken.ThrowIfCancellationRequested();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            return await queryable.CountAsync(cancellationToken);
        }

        public virtual async Task<bool> Any(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> queryable = dbSet.AsNoTracking();
            cancellationToken.ThrowIfCancellationRequested();
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            return (await queryable.CountAsync(cancellationToken)) > 0;
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
