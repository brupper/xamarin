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
    }
}
