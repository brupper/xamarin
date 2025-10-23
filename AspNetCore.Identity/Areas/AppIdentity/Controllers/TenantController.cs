using AutoMapper;
using Brupper.AspNetCore.Exceptions;
using Brupper.AspNetCore.Identity.Entities;
using Brupper.AspNetCore.Identity.Models;
using Brupper.AspNetCore.Identity.Repositories;
using Brupper.AspNetCore.Identity.Resources;
using Brupper.AspNetCore.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

[Authorize(Services.Users.IdentityConstants.Roles.SuperAdmin)]
public class TenantController : ACrudController<ITenantRepository, Tenant, TenantViewModel, TenantListViewModel, TenantViewModel, TenantViewModel, TenantViewModel>
{
    public static IEnumerable<TenantModulesViewModel> DefaultModules => new List<TenantModulesViewModel>()
    {
        new() { Name = "Foglaláskezelés", },
        new() { Name = "Számlázás", },
        new() { Name = "CRM", },
    };

    #region Fields

    private readonly IUserService userService;

    #endregion

    #region Constructor

    public TenantController(
        IUserService userService,
        ITenantRepository tenantRepository,
        IMapper mapper,
        IStringLocalizer<Labels> stringLocalizer,
        ILogger<TenantController> logger)
        : base(tenantRepository, mapper, stringLocalizer, logger)
    {
        this.userService = userService;
    }

    #endregion

    #region Actions

    #endregion

    protected override async Task InternalExecuteDeleteAsync(Tenant entity)
    {
        //IUserStore<User> userStore;

        var usersOfTenant = await userService.GetByTenantIdAsync(entity.Id);

        if (usersOfTenant.Any()) // TBD mindent torulunk???
        {
            var msg = "Tenant could not be deleted due to it has multiple user referenced.";
            throw DomainExceptionFactory.CreateOperationFailed(msg, msg);
        }

        foreach (var user in usersOfTenant)
        {
            user.TenantId = null;
        }

        await repository.DeleteAsync(entity);
    }

    protected override async Task<Tenant> InternalExecuteEditAsync(TenantViewModel model)
    {
        var result = await CreateIfNotExistsAsync(model.PrimaryKey);
        var entity = result.Entity;

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

        entity.Licences = new();
        await repository.UpdateAsync(entity);

        var modules = model.Modules.Where(x => x.Selected).ToList();

        entity.Licences = modules.Select(x => new ModuleReference { Name = x.Name }).ToList();
        await repository.UpdateAsync(entity);
        await repository.SaveAsync();

        await userService.SetupLicencesAsync(entity.Id, modules);

        return entity;
    }

    protected override async Task<TenantViewModel> GetEditViewModel(Tenant? entity)
    {
        var model = await base.GetEditViewModel(entity);

        var wholeModuleList = DefaultModules.ToList();

        wholeModuleList.ForEach(x => x.Selected = model.Modules.Any(m => m.Name == x.Name));

        model.Modules = wholeModuleList;

        return model;
    }
}
