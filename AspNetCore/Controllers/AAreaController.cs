using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brupper.AspNetCore.Controllers;

[Authorize]
public abstract class AAreaController : Controller
{
    protected readonly ILogger logger;
    protected readonly IMapper mapper;

    #region Constructor

    protected AAreaController(
        IMapper mapper,
        ILogger logger)
    {
        this.mapper = mapper;
        this.logger = logger;
    }

    #endregion

    public virtual string Email => User.GetEmail();
    public virtual string UserId => User.GetUserId();
}
