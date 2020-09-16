using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Brupper.Identity.B2C.Models
{
    public class GraphUser : Entity
    {
        #region Fields

        private string givenName;
        private string surName;
        private string state;
        private string postalCode;
        private string country;
        private List<string> otherMails = new List<string>();
        private string streetAddress;
        private string city;
        private ProfileData profileData = new ProfileData();

        #endregion

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "profileData", Required = Newtonsoft.Json.Required.Default)]
        public ProfileData ProfileData { get => profileData; set => SetProperty(ref profileData, value); }

        /// <summary> Gets or sets given name. The given name (first name) of the user. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "givenName", Required = Newtonsoft.Json.Required.Default)]
        public string GivenName { get => givenName; set => SetProperty(ref givenName, value); }

        /// <summary> Gets or sets surname. The user's surname (family name or last name). Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "surname", Required = Newtonsoft.Json.Required.Default)]
        public string Surname { get => surName; set => SetProperty(ref surName, value); }

        /// <summary> Gets or sets state. The state or province in the user's address. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state", Required = Newtonsoft.Json.Required.Default)]
        public string State { get => state; set => SetProperty(ref state, value); }

        /// <summary> Gets or sets postal code. The postal code for the user's postal address. The postal code is specific to the user's country/region. In the United States of America, this attribute contains the ZIP code.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "postalCode", Required = Newtonsoft.Json.Required.Default)]
        public string PostalCode { get => postalCode; set => SetProperty(ref postalCode, value); }

        /// <summary> Gets or sets country. The country/region in which the user is located; for example, 'US' or 'UK'. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "country", Required = Newtonsoft.Json.Required.Default)]
        public string Country { get => country; set => SetProperty(ref country, value); }

        /// <summary> Gets or sets street address. The street address of the user's place of business.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "streetAddress", Required = Newtonsoft.Json.Required.Default)]
        public string StreetAddress { get => streetAddress; set => SetProperty(ref streetAddress, value); }

        /// <summary> Gets or sets city. The city in which the user is located. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "city", Required = Newtonsoft.Json.Required.Default)]
        public string City { get => city; set => SetProperty(ref city, value); }

        /// <summary> Gets or sets other mails. A list of additional email addresses for the user; for example: ['bob@contoso.com', 'Robert@fabrikam.com']. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "otherMails", Required = Newtonsoft.Json.Required.Default)]
        public List<string> OtherMails { get => otherMails; set => SetProperty(ref otherMails, value); }

        [JsonIgnore]
        public string EmailAddress
        {
            get => OtherMails?.FirstOrDefault();
            // READONLY - came by registration  set { OtherMails.Add(value); RaisePropertyChanged(); }
        }
    }
}
