using Brupper.AspNetCore.Identity.Entities;
using AspNetCore.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;

public class IdentityDataContext(DbContextOptions<IdentityDataContext> options)
    : CosmosIdentityDbContext<User, IdentityRole, string>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //  https://aka.ms/ef-cosmos-nosync
        optionsBuilder.ConfigureWarnings(w => w.Ignore(CosmosEventId.SyncNotSupported));
    }
}
