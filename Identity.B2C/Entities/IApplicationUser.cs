namespace Brupper.Identity.B2C.Entities
{
    public interface IApplicationUser
    {
        string Email { get; set; }

        string UserName { get; set; }

        string PhoneNumber { get; set; }

        string CompanyName { get; set; }

        string Country { get; set; }

        string State { get; set; }

        string City { get; set; }

        string PostalCode { get; set; }

        string StreetAddress { get; set; }

        string UserType { get; set; }

        string VatNumber { get; set; }

        string BankAccount { get; set; }
    }
}