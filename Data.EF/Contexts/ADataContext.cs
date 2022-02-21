using Microsoft.EntityFrameworkCore;
using System;

namespace Brupper.Data.EF.Contexts
{
    /// <summary>
    /// Migration init memo:
    /// Install-Package Microsoft.EntityFrameworkCore.Tools
    /// Add-Migration InitialCreate
    /// Update-Database
    /// </summary>
    public abstract class ADataContext : DbContext
    {
        #region Constructors

        public ADataContext(DbContextOptions options) : base(options)
        { }

        protected ADataContext()
        { }

        #endregion

        public virtual void SetModified(object entity)
        {
            if (entity != null)
            {
                Entry(entity).State = EntityState.Modified;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<>(i =>
            //{
            //    i.HasIndex(x => x.Id);
            //    //i.HasOne<User>()    // <---
            //    //    .WithMany()     // <---
            //    //    .HasForeignKey(c => c.UserId);
            //});

            base.OnModelCreating(modelBuilder);
        }

        //public void DetachAllEntities()
        //{
        //    var changedEntriesCopy = context.ChangeTracker.Entries()
        //        .Where(e => e.State == EntityState.Added ||
        //                    e.State == EntityState.Modified ||
        //                    e.State == EntityState.Deleted)
        //        .ToList();
        //
        //    foreach (var entry in changedEntriesCopy)
        //        entry.State = EntityState.Detached;
        //}
    }
}
