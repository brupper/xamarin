namespace B2C
{
    public interface IB2CSettings
    {
        /// <summary> = "pfkb2c"; </summary>
        string TenantPrefix { get; }

        /// <summary> = $"{TenantPrefix}.onmicrosoft.com"; </summary>
        string Tenant { get; }

        /// <summary> = $"{TenantPrefix}.b2clogin.com"; </summary>
        string AzureADB2CHostname { get; }

        /// <summary> = "a5f522b8-b0d4-45d8-9da2-cbebd4590624"; </summary>
        string ClientID { get; }

        /// <summary>= "B2C_1_test"; </summary>
        string PolicySignUpSignIn { get; }

        /// <summary> = "b2c_1_edit_profile"; </summary>
        string PolicyEditProfile { get; }

        /// <summary> = "b2c_1_reset"; </summary>
        string PolicyResetPassword { get; }

        /// <summary> = new []{ $"{Tenant}/api/read", "openid", "offline_access" }; </summary>
        string[] Scopes { get; }

        string AuthorityBase { get; }
        string AuthoritySignInSignUp { get; }
        string AuthorityEditProfile { get; }
        string AuthorityPasswordReset { get; }

        string InAppRedirectUri { get; }

        /// <summary> = "https://{AzureADB2CHostname}/{Tenant}/{PolicySignUpSignIn}/v2.0/.well-known/openid-configuration"; </summary>
        string StsDiscoveryEndpoint { get; }
    }

    public abstract class AB2CSettings : IB2CSettings
    {
        /// <inheritdoc/>
        public abstract string TenantPrefix { get; }

        /// <inheritdoc/>
        public virtual string Tenant
            => $"{TenantPrefix}.onmicrosoft.com";

        /// <inheritdoc/>
        public virtual string AzureADB2CHostname
            => $"{TenantPrefix}.b2clogin.com";

        /// <inheritdoc/>
        public abstract string ClientID { get; }

        /// <inheritdoc/>
        public abstract string PolicySignUpSignIn { get; }

        /// <inheritdoc/>
        public abstract string PolicyEditProfile { get; }

        /// <inheritdoc/>
        public abstract string PolicyResetPassword { get; }

        public abstract string InAppRedirectUri { get; }

        /// <inheritdoc/>
        public virtual string[] Scopes
            => new[] { $"https://{Tenant}/api/read", "openid", "offline_access" };

        /// <inheritdoc/>
        public virtual string StsDiscoveryEndpoint
            => $"https://{AzureADB2CHostname}/{Tenant}/{PolicySignUpSignIn}/v2.0/.well-known/openid-configuration";

        /// <inheritdoc/>
        public virtual string AuthorityBase
            => $"https://{AzureADB2CHostname}/tfp/{Tenant}/";

        /// <inheritdoc/>
        public virtual string AuthoritySignInSignUp
            => $"{AuthorityBase}{PolicySignUpSignIn}";

        /// <inheritdoc/>
        public virtual string AuthorityEditProfile
            => $"{AuthorityBase}{PolicyEditProfile}";

        /// <inheritdoc/>
        public virtual string AuthorityPasswordReset
            => $"{AuthorityBase}{PolicyResetPassword}";

        public virtual string GetPasswordResetUrl()
        {
            return $"https://{AzureADB2CHostname}/{Tenant}/oauth2/v2.0/authorize?p={PolicyResetPassword}&client_id={ClientID}&nonce=defaultNonce&redirect_uri={InAppRedirectUri}&scope=openid&response_type=id_token&prompt=login";
        }
    }
}
