using System.ComponentModel.DataAnnotations;

namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Models;

public class HasAddressViewModel
{
    #region IAddress

    /// <inheritdoc/>
    [/* Required, #8386 */Display(Name = nameof(Resources.Labels.customer_address_zip), ResourceType = typeof(Resources.Labels))]
    public virtual string? Zip { get; set; } = default!;

    /// <inheritdoc/>
    [/* Required, #8386 */Display(Name = nameof(Resources.Labels.customer_address_city), ResourceType = typeof(Resources.Labels))]
    public virtual string? City { get; set; } = default!;

    /// <inheritdoc/>
    [/* Required, #8386 */Display(Name = nameof(Resources.Labels.customer_address_street), ResourceType = typeof(Resources.Labels))]
    public virtual string? Address { get; set; } = default!;

    /// <inheritdoc/>
    [/* Required, #8386 */Display(Name = nameof(Resources.Labels.customer_address_streetnumber), ResourceType = typeof(Resources.Labels))]
    public virtual string? Number { get; set; } = default!;

    /// <inheritdoc/>
    [Display(Name = nameof(Resources.Labels.customer_postaladdress_zip), ResourceType = typeof(Resources.Labels))]
    public virtual string? PostalZip { get; set; }

    /// <inheritdoc/>
    [Display(Name = nameof(Resources.Labels.customer_postaladdress_city), ResourceType = typeof(Resources.Labels))]
    public virtual string? PostalCity { get; set; }

    /// <inheritdoc/>
    [Display(Name = nameof(Resources.Labels.customer_postaladdress_address), ResourceType = typeof(Resources.Labels))]
    public virtual string? PostalAddress { get; set; }

    /// <inheritdoc/>
    [Display(Name = nameof(Resources.Labels.customer_address_streetnumber), ResourceType = typeof(Resources.Labels))]
    public virtual string? PostalNumber { get; set; } = default!;

    #endregion

    public bool HasSamePostalAddress() => Number == PostalNumber && Address == PostalAddress && Zip == PostalZip && City == PostalCity;
}
