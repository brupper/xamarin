// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Graph;
using Newtonsoft.Json;

namespace Brupper.AspNetCore.Identity.B2C;

public class UserModel : User
{
    [JsonProperty(PropertyName = "password", NullValueHandling = NullValueHandling.Ignore)]
    public string? Password { get; set; }

    public void SetB2CProfile(string TenantName)
    {
        PasswordProfile = new PasswordProfile
        {
            ForceChangePasswordNextSignIn = false,
            Password = Password,
            ODataType = null
        };
        PasswordPolicies = "DisablePasswordExpiration,DisableStrongPassword";
        Password = null;
        ODataType = null;

        foreach (var item in Identities)
        {
            if (item.SignInType == "emailAddress" || item.SignInType == "userName")
            {
                item.Issuer = TenantName;
            }
        }
    }

    public override string ToString()
        => JsonConvert.SerializeObject(this);
}