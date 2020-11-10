using Brupper.Identity.B2C.Models;
using Newtonsoft.Json;

namespace Brupper.Identity.B2C.Entities
{
    public class ApplicationUser : NotifyPropertyChanged, IApplicationUser
    {
        #region Fields

        private string userId;
        private string email;
        private string userName;
        private string phoneNumber;
        private string companyName;

        private string country = "HU";
        private string state;
        private string postalCode;
        private string city;
        private string streetAddress;

        private string userType;
        private string vatNumber;
        private string bankAccount;

        #endregion

        /// <summary> . </summary>
        public virtual string UserId { get => userId; set => SetProperty(ref userId, value); }

        /// <summary> . </summary>
        public virtual string Email { get => email; set => SetProperty(ref email, value); }

        /// <summary> . </summary>
        public virtual string UserName { get => userName; set => SetProperty(ref userName, value); }

        /// <summary> Optional </summary>
        public virtual string PhoneNumber { get => phoneNumber; set => SetProperty(ref phoneNumber, value); }

        /// <summary> Optional </summary>
        public virtual string CompanyName { get => companyName; set => SetProperty(ref companyName, value); }

        #region Address

        /// <summary> Gets or sets country. The country/region in which the user is located; for example, 'US' or 'UK'. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "country", Required = Newtonsoft.Json.Required.Default)]
        public string Country { get => country; set => SetProperty(ref country, value); }

        /// <summary> Gets or sets state. The state or province in the user's address. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state", Required = Newtonsoft.Json.Required.Default)]
        public string State { get => state; set => SetProperty(ref state, value); }

        /// <summary> Gets or sets city. The city in which the user is located. Supports $filter.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "city", Required = Newtonsoft.Json.Required.Default)]
        public string City { get => city; set => SetProperty(ref city, value); }

        /// <summary> Gets or sets postal code. The postal code for the user's postal address. The postal code is specific to the user's country/region. In the United States of America, this attribute contains the ZIP code.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "postalCode", Required = Newtonsoft.Json.Required.Default)]
        public string PostalCode { get => postalCode; set => SetProperty(ref postalCode, value); }

        /// <summary> Gets or sets street address. The street address of the user's place of business.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "streetAddress", Required = Newtonsoft.Json.Required.Default)]
        public string StreetAddress { get => streetAddress; set => SetProperty(ref streetAddress, value); }

        #endregion

        /// <summary> . </summary>
        public virtual string UserType { get => userType; set => SetProperty(ref userType, value); }

        /// <summary> . </summary>
        public virtual string VatNumber { get => vatNumber; set => SetProperty(ref vatNumber, value); }

        /// <summary> Optional </summary>
        public virtual string BankAccount { get => bankAccount; set => SetProperty(ref bankAccount, value); }
    }
}
