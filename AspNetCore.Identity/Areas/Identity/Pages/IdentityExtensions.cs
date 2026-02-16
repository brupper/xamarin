using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Brupper.AspNetCore.Identity.Areas.Identity;

public static class IdentityExtensions
{
    public static bool Process(this IdentityResult result, ModelStateDictionary modelState)
    {
        foreach (IdentityError err in result.Errors ?? Enumerable.Empty<IdentityError>())
        {
            modelState.AddModelError(string.Empty, err.Description);
        }

        return result.Succeeded;
    }
}
