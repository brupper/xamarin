using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Brupper.AspNetCore;

public static class IdentityExtensions
{
    public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public static string GetEmail(this ClaimsPrincipal User) => User?.Identity?.Name ?? User?.Claims?.FirstOrDefault(x => x.Type == "emails")?.Value ?? "unknown";

    public static string GetUserId(this ClaimsPrincipal User) => User?.Claims?.FirstOrDefault(x => x.Type == ObjectIdentifierType)?.Value ?? "unknown";

    public static string[] GetUserLanguages(this HttpRequest request) => request.GetTypedHeaders()
            .AcceptLanguage
            ?.OrderByDescending(x => x.Quality ?? 1)
            .Select(x => x.Value.ToString())
            .ToArray() ?? Array.Empty<string>();
}
