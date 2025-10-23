using Brupper.AspNetCore.Identity.Contexts;
using Brupper.AspNetCore.Identity.Entities;
using Brupper.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Brupper.AspNetCore.Identity.Repositories;

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
