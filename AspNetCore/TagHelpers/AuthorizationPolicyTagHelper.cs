using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Brupper.AspNetCore.TagHelpers;

/*
<div asp-authorize asp-roles="Admin" asp-policy="Seniors" class="panel panel-default">
    <div class="panel-heading">Admin Seniors Only</div>
    <div class="panel-body">
        Only users who have both the Admin role AND are age 65 or older can see this section.
    </div>
</div>
*/
/// <summary> https://www.davepaquette.com/archive/2017/11/05/authorize-tag-helper.aspx </summary>
[HtmlTargetElement(Attributes = "asp-authorize")]
[HtmlTargetElement(Attributes = "asp-authorize,asp-policy")]
[HtmlTargetElement(Attributes = "asp-authorize,asp-roles")]
[HtmlTargetElement(Attributes = "asp-authorize,asp-authentication-schemes")]
public class AuthorizationPolicyTagHelper : TagHelper, IAuthorizeData
{
    private readonly IAuthorizationPolicyProvider policyProvider;
    private readonly IPolicyEvaluator policyEvaluator;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthorizationPolicyTagHelper(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationPolicyProvider policyProvider,
        IPolicyEvaluator policyEvaluator)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.policyProvider = policyProvider;
        this.policyEvaluator = policyEvaluator;
    }

    /// <summary>
    /// Gets or sets the policy name that determines access to the HTML block.
    /// </summary>
    [HtmlAttributeName("asp-policy")]
    public string Policy { get; set; }

    /// <summary>
    /// Gets or sets a comma delimited list of roles that are allowed to access the HTML  block.
    /// </summary>
    [HtmlAttributeName("asp-roles")]
    public string Roles { get; set; }

    /// <summary>
    /// Gets or sets a comma delimited list of schemes from which user information is constructed.
    /// </summary>
    [HtmlAttributeName("asp-authentication-schemes")]
    public string AuthenticationSchemes { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var policy = await AuthorizationPolicy.CombineAsync(policyProvider, new[] { this });

        var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, httpContextAccessor.HttpContext);

        var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, httpContextAccessor.HttpContext, null);

        if (!authorizeResult.Succeeded)
        {
            output.SuppressOutput();
        }
    }
}
