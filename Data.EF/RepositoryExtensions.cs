using Brupper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Brupper.Data.EF
{
    public static class RepositoryExtensions
    {
        public static IQueryable<TEntity> SafeInclude<TEntity>(this IQueryable<TEntity> query, string includeProperties = "")
            where TEntity : class, Entities.IBaseEntity, new()
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }

        public static IQueryable<TEntity> SafeInclude<TEntity>(this DbSet<TEntity> dbSet, string includeProperties = "")
            where TEntity : class, Entities.IBaseEntity, new()
        {
            IQueryable<TEntity> query = dbSet.AsNoTracking();
            return query.SafeInclude(includeProperties);
        }
    }
}
