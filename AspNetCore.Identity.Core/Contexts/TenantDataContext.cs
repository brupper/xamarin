using Brupper.AspNetCore.Identity.Entities;
using Brupper.Data.EF.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Brupper.AspNetCore.Identity.Contexts;

public class TenantDataContext : ADataContext
{
    #region Constructors

    public TenantDataContext(DbContextOptions<TenantDataContext> options)
    : base(options) { }

    public TenantDataContext(DbContextOptions options)
        : base(options)
    { }

    #endregion

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<Tenant> Tenants { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var modelBuilder = builder.Entity<Tenant>();
        modelBuilder.HasKey(o => o.Id);
        modelBuilder.Property(o => o.Id).ValueGeneratedOnAdd();

        var l = modelBuilder.OwnsMany(p => p.Licences);
        // l.HasKey(x => x.Id); // nem unique, nem kell => .HasKey(x => x.Id);

        base.OnModelCreating(builder);
    }
}
