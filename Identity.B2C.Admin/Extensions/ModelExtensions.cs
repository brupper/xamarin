using Brupper.Identity.B2C.Entities;
using Microsoft.Graph;
using Newtonsoft.Json;
using System.Linq;

namespace Brupper.Identity.B2C
{
    public static class ModelExtensions
    {
        public static ApplicationUser CreateFrom(this User source)
        {
            if (source == null)
            {
                //throw new ArgumentNullException("Grap.User.Create: neither source, nor destination can not be NULL");
                return null;
            }

            var destination = JsonConvert.DeserializeObject<ApplicationUser>(JsonConvert.SerializeObject(source));
            destination.UserId = source.Id;
            destination.Email = source.GetMail();
            destination.UserName = $"{source.GivenName} {source.Surname} {source.DisplayName}".TrimStart().TrimEnd();
            destination.PhoneNumber = source.MobilePhone ?? source.BusinessPhones?.FirstOrDefault();
            destination.CompanyName = source.CompanyName;
            destination.Country = source.Country;
            destination.State = source.State;
            destination.City = source.City;
            destination.PostalCode = source.PostalCode;
            destination.StreetAddress = source.StreetAddress;
            destination.UserType = source.UserType;
            destination.VatNumber = string.Empty;
            destination.BankAccount = string.Empty;

            return destination;
        }
    }
}
