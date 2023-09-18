namespace Brupper.AspNetCore.Models;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class AjaxNotifcationModel
{
    public const string Key = "backendnotification";

    public string Message { get; set; } = default!;

    public bool Succeeded { get; set; }

    public static AjaxNotifcationModel Failed(string message) => new AjaxNotifcationModel
    {
        Succeeded = false,
        Message = message
    };

    public static AjaxNotifcationModel Success(string message) => new AjaxNotifcationModel
    {
        Succeeded = true,
        Message = message
    };
}
