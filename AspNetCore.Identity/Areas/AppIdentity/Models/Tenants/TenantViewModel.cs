using System.ComponentModel.DataAnnotations;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

[System.Diagnostics.DebuggerDisplay("{PrimaryKey,nq} - {Name,nq}")]
public class TenantViewModel : HasAddressViewModel
{
    public string PrimaryKey { get; set; } = default!;

    [Required, Display(Name = nameof(Resources.Labels.customer_name), ResourceType = typeof(Resources.Labels))]
    public virtual string Name { get; set; } = default!;

    [Display(Name = nameof(Resources.Labels.customer_contact), ResourceType = typeof(Resources.Labels))]
    public virtual string Contact { get; set; } = default!;

    [Display(Name = nameof(Resources.Labels.phone), ResourceType = typeof(Resources.Labels))]
    public virtual string? Phone { get; set; }

    [Display(Name = nameof(Resources.Labels.email), ResourceType = typeof(Resources.Labels))]
    public virtual string? Email { get; set; }

    [Display(Name = nameof(Resources.Labels.customer_postaladdress_sameasaddress), ResourceType = typeof(Resources.Labels))]
    public virtual bool SamePostalAddress { get; set; }

    [Display(Name = nameof(Resources.Labels.tenant_modules), ResourceType = typeof(Resources.Labels))]
    public List<TenantModulesViewModel> Modules { get; set; } = new();
}

public class TenantModulesViewModel
{
    public string Name { get; set; } = default!;

    public bool Selected { get; set; }
}
