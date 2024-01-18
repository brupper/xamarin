namespace Brupper.AspNetCore.Identity.B2C;

internal class B2cCustomAttributeHelper(string b2cExtensionAppClientId)
{
    internal readonly string _b2cExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");

    internal string GetCompleteAttributeName(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Parameter cannot be null", nameof(attributeName));
        }

        return $"extension_{_b2cExtensionAppClientId}_{attributeName}";
    }
}
