using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using AspNetCore.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;

public class IdentityDataContext(DbContextOptions<IdentityDataContext> options)
    : CosmosIdentityDbContext<User, IdentityRole, string>(options)
{
}
