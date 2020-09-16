using Newtonsoft.Json;
using System;

namespace Brupper.Identity.B2C.Models
{
    public partial class JwtUser
    {
        [JsonProperty("exp")]
        public long Exp { get; set; }

        [JsonProperty("nbf")]
        public long Nbf { get; set; }

        [JsonProperty("ver")]
        public string Ver { get; set; }

        [JsonProperty("iss")]
        public Uri Iss { get; set; }

        [JsonProperty("sub")]
        public Guid Sub { get; set; }

        [JsonProperty("aud")]
        public Guid Aud { get; set; }

        [JsonProperty("iat")]
        public string Iat { get; set; }

        [JsonProperty("auth_time")]
        public long AuthTime { get; set; }

        [JsonProperty("idp_access_token")]
        public string IdpAccessToken { get; set; }

        [JsonProperty("idp")]
        public string Idp { get; set; }

        [JsonProperty("oid")]
        public Guid Oid { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("emails")]
        public string[] Emails { get; set; }
            = new string[0];

        [JsonProperty("tfp")]
        public string Tfp { get; set; }

        [JsonIgnore]
        public string IdToken { get; internal set; }

        [JsonIgnore]
        public bool IsLoggedOn { get; internal set; }
    }
}
