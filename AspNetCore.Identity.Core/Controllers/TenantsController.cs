using AutoMapper;
using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Models;
using Brupper.AspNetCore.Identity.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brupper.AspNetCore.Identity.Controllers;

[Route("api/[area]/[controller]")]
[Authorize(Services.Users.IdentityConstants.Roles.SuperAdmin)]
public class TenantsController(
    IUserService userService,
    ITenantRepository tenantRepository,
    IMapper mapper,
    ILogger<TenantsController> logger)
    : BaseController
{
    public static IEnumerable<TenantModulesViewModel> DefaultModules => new List<TenantModulesViewModel>()
    {
        new() { Name = "Foglaláskezelés", },
        new() { Name = "Számlázás", },
        new() { Name = "CRM", },
    };

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantViewModel>>> GetAllAsync()
    {
        var entities = await tenantRepository.GetAsync();
        var models = mapper.Map<IEnumerable<TenantViewModel>>(entities);
        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantViewModel>> GetByIdAsync(string id)
    {
        var entity = await tenantRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var model = mapper.Map<TenantViewModel>(entity);
        var wholeModuleList = DefaultModules.ToList();
        wholeModuleList.ForEach(x => x.Selected = model.Modules.Any(m => m.Name == x.Name));
        model.Modules = wholeModuleList;

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult<TenantViewModel>> CreateAsync([FromBody] TenantViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = await SaveTenantAsync(model);
        var result = mapper.Map<TenantViewModel>(entity);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TenantViewModel>> UpdateAsync(string id, [FromBody] TenantViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != model.PrimaryKey)
        {
            return BadRequest("ID mismatch");
        }

        var entity = await SaveTenantAsync(model);
        var result = mapper.Map<TenantViewModel>(entity);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var entity = await tenantRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var usersOfTenant = await userService.GetByTenantIdAsync(entity.Id);

        if (usersOfTenant.Any())
        {
            return BadRequest("Tenant could not be deleted due to it has multiple user referenced.");
        }

        await tenantRepository.DeleteAsync(entity);
        await tenantRepository.SaveAsync();

        return NoContent();
    }

    private async Task<Tenant> SaveTenantAsync(TenantViewModel model)
    {
        var entity = await tenantRepository.GetByIdAsync(model.PrimaryKey);
        if (entity == null)
        {
            entity = new Tenant { Id = model.PrimaryKey ?? Guid.NewGuid().ToString() };
            await tenantRepository.InsertAsync(entity);
        }

        entity.Name = model.Name;
        entity.Contact = model.Contact;
        entity.Zip = model.Zip;
        entity.City = model.City;
        entity.Address = model.Address;
        entity.Number = model.Number;

        if (model.SamePostalAddress)
        {
            entity.PostalZip = model.Zip;
            entity.PostalCity = model.City;
            entity.PostalAddress = model.Address;
            entity.PostalNumber = model.Number;
        }
        else
        {
            entity.PostalZip = model.PostalZip;
            entity.PostalCity = model.PostalCity;
            entity.PostalAddress = model.PostalAddress;
            entity.PostalNumber = model.PostalNumber;
        }

        entity.Phone = model.Phone;
        entity.Email = model.Email;

        var modules = model.Modules.Where(x => x.Selected).ToList();
        entity.Licences = modules.Select(x => new ModuleReference { Name = x.Name }).ToList();

        await tenantRepository.UpdateAsync(entity);
        await tenantRepository.SaveAsync();

        await userService.SetupLicencesAsync(entity.Id, modules);

        return entity;
    }
}
