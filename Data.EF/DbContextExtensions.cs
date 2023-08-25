using Brupper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Brupper.Data.EF
{
    public static class DbContextExtensions
    {
        public static void DetachLocal<TEntity>(this DbContext context, TEntity t) where TEntity : BaseEntity
        {
            var local = context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(t.Id));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }

            context.SetModified(t);
        }

        public static void DetachAllEntities(this DbContext context)
        {
            var changedEntriesCopy = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
            {
                entry.State = EntityState.Detached;
            }
        }

        public static void SetModified(this DbContext context, object entity)
        {
            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
