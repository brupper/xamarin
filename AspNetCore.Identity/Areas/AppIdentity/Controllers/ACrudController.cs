using Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;
using AutoMapper;
using Brupper.AspNetCore.Models;
using Brupper.Data;
using Brupper.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Controllers;

[Authorize]
[Area(AreaConstants.AreaName)]
public abstract class ACrudController<TRepository, TEntity, TViewModel, TListViewModel, TListItemViewModel, TEditViewModel, TDetailsViewModel>
    : Brupper.AspNetCore.Controllers.ACrudController<TRepository, TEntity, TViewModel, TListViewModel, TListItemViewModel, TEditViewModel, TDetailsViewModel>
    where TEntity : EntityAggregate, new()
    where TViewModel : class, new()
    where TListItemViewModel : class, new()
    where TListViewModel : ListViewModel<TListItemViewModel>, new()
    where TEditViewModel : class, new()
    where TDetailsViewModel : class, new()
    where TRepository : IRepository<TEntity>
{
    #region Constructor

    protected ACrudController(
        TRepository repository,
        IMapper mapper,
        IStringLocalizer stringLocalizer,
        ILogger logger) : base(repository, mapper, stringLocalizer, logger)
    { }

    #endregion
}
