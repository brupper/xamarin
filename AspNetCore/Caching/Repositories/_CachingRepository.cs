using Brupper.Data.EF;
using Brupper.Data.EF.Contexts;
using Brupper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Brupper.AspNetCore.Caching.Repositories;

public abstract class CachingRepository<TEntity>(ADataContext context, ICacheService cacheService) : Repository<TEntity>(context)
    where TEntity : BaseEntity
{
    protected abstract string CachingKey { get; }

    /*
    // dbSet.AsNoTracking() => helytelenul van hasznalva az ososztalyban...???;
    // */
    public override async Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int? pageSize = null,
        int pageNumber = 0,
        string includeProperties = "",
        CancellationToken cancellationToken = default)
    {
        var query = tracking ? (IQueryable<TEntity>)dbSet : dbSet.AsNoTracking();

        query = (await cacheService.GetOrAddAsync(
                CachingKey,
                async () => (await query.ToListAsync(cancellationToken: cancellationToken)).ToList(), DateTimeOffset.Now.AddDays(30)))
            .AsQueryable();

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

        return query.ToList(); // NOT ToListAsync(cancellationToken)
    }

    public override async Task UpdateAsync(TEntity entityToUpdate)
    {
        await base.UpdateAsync(entityToUpdate);
        cacheService.Remove(CachingKey);
    }

    public override async Task InsertAsync(TEntity entityToUpdate)
    {
        await base.InsertAsync(entityToUpdate);
        cacheService.Remove(CachingKey);
    }

    public override async Task DeleteAsync(TEntity entityToDelete)
    {
        if (entityToDelete == null)
        {
            return;
        }

        await base.DeleteAsync(entityToDelete);
        cacheService.Remove(CachingKey);
    }
}
