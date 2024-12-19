using Brupper.AspNetCore.Identity.Areas.AppIdentity.Contexts;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;
using Brupper.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(TenantDataContext context) : base(context) { }

    public override async Task DeleteAsync(Tenant entity)
    {
        var company = await dbSet.FirstOrDefaultAsync(x => x.Id == entity.Id) ?? throw new TenantNotFoundException(entity.Id);

        company.Deleted = true;
        // dbSet.Remove(company);

        await SaveAsync();
    }
}
