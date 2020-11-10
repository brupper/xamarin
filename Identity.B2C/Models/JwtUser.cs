using Newtonsoft.Json;
using System;

namespace Brupper.Identity.B2C.Models
{
    /// <summary> . </summary>
    public partial class JwtUser
    {
        /// <summary> . </summary>
        [JsonProperty("exp")]
        public long Exp { get; set; }

        /// <summary> . </summary>
        [JsonProperty("nbf")]
        public long Nbf { get; set; }

        /// <summary> . </summary>
        [JsonProperty("ver")]
        public string Ver { get; set; }

        /// <summary> . </summary>
        [JsonProperty("iss")]
        public Uri Iss { get; set; }

        /// <summary> . </summary>
        [JsonProperty("sub")]
        public Guid Sub { get; set; }

        /// <summary> . </summary>
        [JsonProperty("aud")]
        public Guid Aud { get; set; }

        /// <summary> . </summary>
        [JsonProperty("iat")]
        public string Iat { get; set; }

        /// <summary> . </summary>
        [JsonProperty("auth_time")]
        public long AuthTime { get; set; }

        /// <summary> . </summary>
        [JsonProperty("idp_access_token")]
        public string IdpAccessToken { get; set; }

        /// <summary> . </summary>
        [JsonProperty("idp")]
        public string Idp { get; set; }

        /// <summary> . </summary>
        [JsonProperty("oid")]
        public Guid Oid { get; set; }

        /// <summary> . </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary> . </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary> . </summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary> . </summary>
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        /// <summary> . </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary> . </summary>
        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        /// <summary> . </summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        /// <summary> . </summary>
        [JsonProperty("emails")]
        public string[] Emails { get; set; }
            = new string[0];

        /// <summary> . </summary>
        [JsonProperty("tfp")]
        public string Tfp { get; set; }

        /// <summary> . </summary>
        [JsonIgnore]
        public string IdToken { get; internal set; }

        /// <summary> . </summary>
        [JsonIgnore]
        public bool IsLoggedOn { get; internal set; }
    }
}
