using AutoMapper;
using Brupper.AspNetCore.Exceptions;
using Brupper.AspNetCore.Models;
using Brupper.AspNetCore.Resources;
using Brupper.Data;
using Brupper.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Brupper.AspNetCore.Controllers;


[Authorize]
public abstract class ACrudController<TRepository, TEntity, TViewModel, TListViewModel, TListItemViewModel, TEditViewModel, TDetailsViewModel>
    : ACrudTypedKeyController<TRepository, TEntity, TViewModel, TListViewModel, TListItemViewModel, TEditViewModel, TDetailsViewModel, string>
    where TEntity : BaseEntity, new()
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
        //IStringLocalizer<SharedResource> sharedLocalizer,
        IStringLocalizer localizer,
        ILogger logger)
        : base(repository, mapper, localizer, logger)
    {
    }

    #endregion
}

[Authorize]
public abstract class ACrudTypedKeyController
    <TRepository, TEntity, TViewModel, TListViewModel, TListItemViewModel, TEditViewModel, TDetailsViewModel, TKeyType>
    : AAreaController
    where TEntity : BaseEntity, new()
    where TViewModel : class, new()
    where TListItemViewModel : class, new()
    where TListViewModel : ListViewModel<TListItemViewModel>, new()
    where TEditViewModel : class, new()
    where TDetailsViewModel : class, new()
    where TRepository : IRepository<TEntity>
{
    protected readonly TRepository repository;
    protected readonly IStringLocalizer localizer;

    #region Constructor

    protected ACrudTypedKeyController(
        TRepository repository,
        IMapper mapper,
        //IStringLocalizer<SharedResource> sharedLocalizer,
        IStringLocalizer localizer,
        ILogger logger) : base(mapper, logger)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    #endregion

    #region Actions

    public virtual async Task<IActionResult> Index()
    {
        var model = new TListViewModel();

        try
        {
            var items = await repository.GetAsync();

            model = await GetListViewModelAsync(items);
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_load_failed"], ex)));
        }

        return View(model);
    }

    public virtual async Task<IActionResult> Details(TKeyType id)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_entity_notfound"], id)));
                return RedirectToAction(nameof(Index));
            }

            var model = await GetDetailsViewModel(entity);

            return View(model);
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_load_failed"], ex)));
            return RedirectToAction(nameof(Index));
        }
    }

    public virtual async Task<IActionResult> Create()
    {
        try
        {
            var model = await GetEditViewModel(null);

            return View("Edit", model);
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_load_failed"], ex)));
            return RedirectToAction(nameof(Index));
        }
    }

    public virtual async Task<IActionResult> Edit(TKeyType id)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_entity_notfound"], id)));
                return RedirectToAction(nameof(Index));
            }

            var model = await GetEditViewModel(entity);

            return View(model);
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_load_failed"], ex)));
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    //public virtual async Task<IActionResult> Edit(IFormCollection form)
    // string id, [FromBody]
    public virtual async Task<IActionResult> Edit(bool? continueEditing, TEditViewModel model)
    {
        var id = Request.Path.HasValue ? Request.Path.Value.Split('/')?.LastOrDefault() : string.Empty;
        var entityExists = false;

        try
        {
            var entity = await repository.GetByIdAsync(id);
            entityExists = entity != null;                  // we need to know if it does exist. Does matter the request come from Create or Edit

            entity = await InternalExecuteEditAsync(model);
            id = entity.Id; // safety first (at create forms we do not have value of field id)

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(localizer["general_save_succes"]));
        }
        catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], e.ClientMessage)));
            return entityExists
                ? RedirectToAction(nameof(Edit), new { id = id })
                : RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_save_failed"], ex)));
            return entityExists
                ? RedirectToAction(nameof(Edit), new { id = id })
                : RedirectToAction(nameof(Index));
        }

        if (continueEditing.HasValue && continueEditing.Value)
        {
            return RedirectToAction(nameof(Edit), new { id = id });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Delete(GeneralDeleteViewModel model)
    {
        try
        {
            var entity = await repository.GetByIdAsync(model.PrimaryKey);
            await InternalExecuteDeleteAsync(entity);

            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Success(localizer["general_delete_succes"]));
        }
        catch (DomainException e) when (e.Type == DomainExceptionType.OperationFailed)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_delete_failed"], e.ClientMessage)));
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_delete_failed"], ex)));
        }

        return RedirectToAction(nameof(Index));
    }

    #region Popups / Dialogs / OffCanvases

    public virtual async Task<IActionResult> CreateForm()
    {
        try
        {
            var model = await GetEditViewModel(await repository.GetByIdAsync(default(TKeyType)));

            return View("_FormCreatePartial", model);
        }
        catch (Exception ex)
        {
            TempData.Put(AjaxNotifcationModel.Key, AjaxNotifcationModel.Failed(string.Format(localizer["general_load_failed"], ex)));
            return View("_FormCreatePartial", null);
        }
    }

    #endregion

    #endregion

    protected virtual Task InternalExecuteDeleteAsync(TEntity entity)
    {
        return repository.DeleteAsync(entity);
    }

    protected abstract Task<TEntity> InternalExecuteEditAsync(TEditViewModel model);

    // protected virtual Task<TCreateViewModel> GetCreateViewModel(TEntity entity) => Task.FromResult<TCreateViewModel>(mapper.Map<TCreateViewModel>(entity));

    protected virtual Task<TListViewModel> GetListViewModelAsync(IEnumerable<TEntity> entities)
        => Task.FromResult<TListViewModel>(
                new TListViewModel() { Items = entities.Select(x => mapper.Map<TListItemViewModel>(x)).ToList() });

    protected virtual Task<TDetailsViewModel> GetDetailsViewModel(TEntity? entity)
        => Task.FromResult<TDetailsViewModel>(
             entity == null
                ? new TDetailsViewModel()
                : mapper.Map<TDetailsViewModel>(entity));

    protected virtual Task<TEditViewModel> GetEditViewModel(TEntity? entity)
        => Task.FromResult<TEditViewModel>(
             entity == null
                ? new TEditViewModel()
                : mapper.Map<TEditViewModel>(entity));

    protected virtual Task<TEntity> GetCreateEntityAsync(TEditViewModel? viewmodel)
        => Task.FromResult<TEntity>(
             viewmodel == null
                ? new TEntity()
                : mapper.Map<TEntity>(viewmodel));

    protected virtual async Task<CreateIfNotExistResult<TEntity>> CreateIfNotExistsAsync(TKeyType id, Func<Task<TEntity>>? createEntity = null)
    {
        var result = new CreateIfNotExistResult<TEntity>();

        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            // stactrace: called from Create action
            entity = createEntity != null ? await createEntity() : new TEntity();
            await repository.InsertAsync(entity);

            result.AlreadyExists = false;
        }
        else
        {
            result.AlreadyExists = true;
        }

        result.Entity = entity;

        return result;
    }
}
