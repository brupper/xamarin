namespace Brupper.AspNetCore.Identity.Areas.AppIdentity.Services.Users;

[Serializable]
public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(string username) : base($"User '{username}' not found")
    {
    }
}
