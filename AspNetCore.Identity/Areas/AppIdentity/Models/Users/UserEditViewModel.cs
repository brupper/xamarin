using System.ComponentModel.DataAnnotations;
using Brupper.AspNetCore.Identity.Areas.AppIdentity.Entities;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

public class UserEditViewModel
{
    public string Id { get; set; } = default!;

    [Required]
    [Display(Name = nameof(Resources.Labels.user_email), ResourceType = typeof(Resources.Labels))]
    public string Email { get; set; } = default!;

    [Required]
    [Display(Name = nameof(Resources.Labels.customer_name), ResourceType = typeof(Resources.Labels))]
    public string Name { get; set; } = default!;

    [Display(Name = nameof(Resources.Labels.user_tenant), ResourceType = typeof(Resources.Labels))]
    public string? TenantId { get; set; }

    public List<Tenant> Tenants { get; set; } = new();

    /// <summary> Gets or sets the date and time, in UTC, when any user lockout ends. </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary> Gets or sets a flag indicating if the user could be locked out. </summary>
    public bool LockoutEnabled { get; set; }
}
