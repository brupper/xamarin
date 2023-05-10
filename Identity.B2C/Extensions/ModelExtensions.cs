using System;

namespace Brupper.Identity.B2C.Entities
{
    public static class ModelExtensions
    {
        public static void CopyTo(this IApplicationUser? source, IApplicationUser? destination)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException("IApplicationUser.CopyFrom: neither source, nor destination can not be NULL");
            }

            destination.Email = source.Email;
            destination.UserName = source.UserName;
            destination.PhoneNumber = source.PhoneNumber;
            destination.CompanyName = source.CompanyName;
            destination.Country = source.Country;
            destination.State = source.State;
            destination.City = source.City;
            destination.PostalCode = source.PostalCode;
            destination.StreetAddress = source.StreetAddress;
            destination.UserType = source.UserType;
            destination.VatNumber = source.VatNumber;
            destination.BankAccount = source.BankAccount;
        }
    }
}
