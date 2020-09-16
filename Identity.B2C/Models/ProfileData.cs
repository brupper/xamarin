namespace Brupper.Identity.B2C.Models
{
    public class ProfileData : NotifyPropertyChanged
    {
        #region Fields

        private string name;
        private string country = "HU";
        private string postalCode;
        private string city;
        private string address;

        private int customerType;
        private string vatNumber;
        private string bankAccount;

        private string phone;
        private string email;

        #endregion

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string Country { get => country; set => SetProperty(ref country, value); }

        public string PostalCode { get => postalCode; set => SetProperty(ref postalCode, value); }

        public string City { get => city; set => SetProperty(ref city, value); }

        public string Address { get => address; set => SetProperty(ref address, value); }

        public int CustomerType { get => customerType; set => SetProperty(ref customerType, value); }

        public string VatNumber { get => vatNumber; set => SetProperty(ref vatNumber, value); }

        /// <summary> Optional </summary>
        public string BankAccount { get => bankAccount; set => SetProperty(ref bankAccount, value); }

        /// <summary> Optional </summary>
        public string Phone { get => phone; set => SetProperty(ref phone, value); }

        /// <summary> Optional </summary>
        public string Email { get => email; set => SetProperty(ref email, value); }
    }
}
