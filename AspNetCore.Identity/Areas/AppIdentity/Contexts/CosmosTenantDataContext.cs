using Brupper.AspNetCore.Identity.Contexts;
using Brupper.AspNetCore.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;

public class CosmosTenantDataContext(DbContextOptions<CosmosTenantDataContext> options)
    : TenantDataContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //  https://aka.ms/ef-cosmos-nosync
        optionsBuilder.ConfigureWarnings(w => w.Ignore(CosmosEventId.SyncNotSupported));
    }

    public DbSet<Tenant> Tenants { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var modelBuilder = builder.Entity<Tenant>();
        modelBuilder.ToContainer("tenants");
        modelBuilder.HasPartitionKey(o => o.PartitionKey);
        modelBuilder.HasKey(o => o.Id);
        modelBuilder.Property(o => o.Id).ValueGeneratedOnAdd();

        var l = modelBuilder.OwnsMany(p => p.Licences);
        // l.HasKey(x => x.Id); // nem unique, nem kell => .HasKey(x => x.Id);

        base.OnModelCreating(builder);
    }
}
