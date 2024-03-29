using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Brupper.Identity.B2C
{
    //https://portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Properties

    // https://github.com/microsoftgraph/msgraph-sdk-dotnet
    // https://docs.microsoft.com/en-us/azure/active-directory-b2c/manage-user-accounts-graph-api

    public class UserGraphManager : IUserGraphManager
    {
        private readonly GraphApiKeyServiceClientCredentials config;
        private readonly GraphServiceClient graphClient;

        public static Expression<Func<User, object>> UserSelector { get; } = (e => new
        {
            e.UserPrincipalName,
            e.OnPremisesUserPrincipalName,
            e.Id,
            e.AdditionalData,
            e.Mail,
            e.OtherMails,// https://docs.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview#create-a-sign-up-or-sign-in-policy
            e.Country,
            e.City,
            e.State,
            e.StreetAddress,
            e.PostalCode,
            e.CompanyName,
            e.GivenName,
            e.DisplayName,
            e.Surname,
            e.Identities,
            e.BusinessPhones,
            e.CreatedDateTime,
        });

        #region Constructor

        public UserGraphManager(GraphApiKeyServiceClientCredentials config)
        {
            this.config = config;

            // Initialize the client credential auth provider
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(config.ClientId)
                .WithTenantId(config.Tenant)
                .WithClientSecret(config.ClientSecret)
                .Build();
            var authProvider = new ClientCredentialProvider(confidentialClientApplication);

            // Set up the Microsoft Graph service client with client credentials
            graphClient = new GraphServiceClient(authProvider);
        }

        #endregion

        public async Task<IEnumerable<User>> ListUsersAsync()
        {
            Debug.WriteLine("Getting list of users...");

            // Get all users (one page)
            var result = await graphClient.Users
                .Request()
                .Select(UserSelector)
                .GetAsync();

            //foreach (var user in result.CurrentPage)
            //{
            //    Debug.WriteLine(JsonConvert.SerializeObject(user));
            //}

            var allUser = result.CurrentPage.ToList();
            while (true)
            {
                if (result.NextPageRequest == null)
                {
                    break;
                }

                result = await result.NextPageRequest.GetAsync();
                allUser.AddRange(result.CurrentPage.ToList());
            }

            return allUser;
        }

        public async Task ListUsersWithCustomAttribute(string b2cExtensionAppClientId)
        {
            if (string.IsNullOrWhiteSpace(b2cExtensionAppClientId))
            {
                throw new ArgumentException("B2cExtensionAppClientId (its Application ID) is missing from appsettings.json. Find it in the App registrations pane in the Azure portal. The app registration has the name 'b2c-extensions-app. Do not modify. Used by AADB2C for storing user data.'.", nameof(b2cExtensionAppClientId));
            }

            // Declare the names of the custom attributes
            const string customAttributeName1 = "FavouriteSeason";
            const string customAttributeName2 = "LovesPets";

            // Get the complete name of the custom attribute (Azure AD extension)
            var helper = new B2cCustomAttributeHelper(b2cExtensionAppClientId);
            var favouriteSeasonAttributeName = helper.GetCompleteAttributeName(customAttributeName1);
            var lovesPetsAttributeName = helper.GetCompleteAttributeName(customAttributeName2);

            Debug.WriteLine($"Getting list of users with the custom attributes '{customAttributeName1}' (string) and '{customAttributeName2}' (boolean)");
            Debug.WriteLine("");

            // Get all users (one page)
            var result = await graphClient.Users
                .Request()
                .Select($"id,displayName,identities,{favouriteSeasonAttributeName},{lovesPetsAttributeName}")
                .GetAsync();

            foreach (var user in result.CurrentPage)
            {
                Debug.WriteLine(JsonConvert.SerializeObject(user));

                // Only output the custom attributes...
                //Debug.WriteLine(JsonConvert.SerializeObject(user.AdditionalData));
            }
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            Debug.WriteLine($"Looking for user with object ID '{userId}'...");

            try
            {
                // Get user by object ID
                var result = await graphClient.Users[userId]
                    .Request()
                    .Select(UserSelector)
                    .GetAsync();


                if (result != null)
                {
                    Debug.WriteLine(JsonConvert.SerializeObject(result));
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<User> GetUserBySignInNameAsync(string userId)
        {
            Debug.WriteLine($"Looking for user with sign-in name '{userId}'...");

            try
            {
                // Get user by sign-in name
                var result = await graphClient.Users
                    .Request()
                    .Filter($"identities/any(c:c/issuerAssignedId eq '{userId}' and c/issuer eq '{config.Tenant}')")
                    .Select(UserSelector)
                    .GetAsync();

                if (result != null)
                {
                    Debug.WriteLine(JsonConvert.SerializeObject(result));
                }

                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task DeleteUserByIdAsync(string userId)
        {
            Debug.WriteLine($"Looking for user with object ID '{userId}'...");

            try
            {
                // Delete user by object ID
                await graphClient.Users[userId]
                   .Request()
                   .DeleteAsync();

                Debug.WriteLine($"User with object ID '{userId}' successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task SetPasswordByUserId(string userId, string password)
        {
            Debug.WriteLine($"Looking for user with object ID '{userId}'...");

            var user = new User
            {
                PasswordPolicies = "DisablePasswordExpiration,DisableStrongPassword",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = password,
                }
            };

            try
            {
                // Update user by object ID
                await graphClient.Users[userId]
                   .Request()
                   .UpdateAsync(user);

                Debug.WriteLine($"User with object ID '{userId}' successfully updated.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //public async Task BulkCreate(GraphServiceClient graphClient)
        //{
        //    // Get the users to import
        //    string appDirectoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    string dataFilePath = Path.Combine(appDirectoryPath, UsersFileName);

        //    // Verify and notify on file existence
        //    if (!System.IO.File.Exists(dataFilePath))
        //    {
        //        Debug.WriteLine($"File '{dataFilePath}' not found.");
        //        return;
        //    }

        //    Debug.WriteLine("Starting bulk create operation...");

        //    // Read the data file and convert to object
        //    UsersModel users = UsersModel.Parse(System.IO.File.ReadAllText(dataFilePath));

        //    foreach (var user in users.Users)
        //    {
        //        user.SetB2CProfile(config.Tenant);

        //        try
        //        {
        //            // Create the user account in the directory
        //            User user1 = await graphClient.Users
        //                            .Request()
        //                            .AddAsync(user);

        //            Debug.WriteLine($"User '{user.DisplayName}' successfully created.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //        }
        //    }
        //}

        //public async Task CreateUserWithCustomAttribute(string b2cExtensionAppClientId, string tenantId)
        //{
        //    if (string.IsNullOrWhiteSpace(b2cExtensionAppClientId))
        //    {
        //        throw new ArgumentException("B2C Extension App ClientId (ApplicationId) is missing in the appsettings.json. Get it from the App Registrations blade in the Azure portal. The app registration has the name 'b2c-extensions-app. Do not modify. Used by AADB2C for storing user data.'.", nameof(b2cExtensionAppClientId));
        //    }

        //    // Declare the names of the custom attributes
        //    const string customAttributeName1 = "FavouriteSeason";
        //    const string customAttributeName2 = "LovesPets";

        //    // Get the complete name of the custom attribute (Azure AD extension)
        //    var helper = new B2cCustomAttributeHelper(b2cExtensionAppClientId);
        //    var favouriteSeasonAttributeName = helper.GetCompleteAttributeName(customAttributeName1);
        //    var lovesPetsAttributeName = helper.GetCompleteAttributeName(customAttributeName2);

        //    Debug.WriteLine($"Create a user with the custom attributes '{customAttributeName1}' (string) and '{customAttributeName2}' (boolean)");

        //    // Fill custom attributes
        //    IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
        //    extensionInstance.Add(favouriteSeasonAttributeName, "summer");
        //    extensionInstance.Add(lovesPetsAttributeName, true);

        //    try
        //    {
        //        // Create user
        //        var result = await graphClient.Users
        //        .Request()
        //        .AddAsync(new User
        //        {
        //            GivenName = "Casey",
        //            Surname = "Jensen",
        //            DisplayName = "Casey Jensen",
        //            Identities = new List<ObjectIdentity>
        //            {
        //                new ObjectIdentity()
        //                {
        //                    SignInType = "emailAddress",
        //                    Issuer = tenantId,
        //                    IssuerAssignedId = "casey.jensen@example.com"
        //                }
        //            },
        //            PasswordProfile = new PasswordProfile()
        //            {
        //                Password = PasswordHelper.GenerateNewPassword(4, 8, 4)
        //            },
        //            PasswordPolicies = "DisablePasswordExpiration",
        //            AdditionalData = extensionInstance
        //        });

        //        string userId = result.Id;

        //        Debug.WriteLine($"Created the new user. Now get the created user with object ID '{userId}'...");

        //        // Get created user by object ID
        //        result = await graphClient.Users[userId]
        //            .Request()
        //            .Select($"id,givenName,surName,displayName,identities,{favouriteSeasonAttributeName},{lovesPetsAttributeName}")
        //            .GetAsync();

        //        if (result != null)
        //        {
        //            Debug.WriteLine($"DisplayName: {result.DisplayName}");
        //            Debug.WriteLine($"{customAttributeName1}: {result.AdditionalData[favouriteSeasonAttributeName].ToString()}");
        //            Debug.WriteLine($"{customAttributeName2}: {result.AdditionalData[lovesPetsAttributeName].ToString()}");
        //            Debug.WriteLine("");
        //            Debug.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        //        }
        //    }
        //    catch (ServiceException ex)
        //    {
        //        if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        //        {
        //            Debug.WriteLine($"Have you created the custom attributes '{customAttributeName1}' (string) and '{customAttributeName2}' (boolean) in your tenant?");
        //            Debug.WriteLine("");
        //            Debug.WriteLine(ex.Message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //}

        public async Task<bool> IsLicenseAdministratorAsync(string userId)
        {
            // https://docs.microsoft.com/en-us/graph/api/directoryrole-list-members?view=graph-rest-1.0&tabs=csharp
            const string licenseAdminTemplateId = "4d6ac14f-3453-41d0-bef9-a3e0c569773a"; // - License Administrator

            var roleAssignments = await graphClient.Users[userId].MemberOf.Request().GetAsync().ConfigureAwait(false);

            return roleAssignments.OfType<DirectoryRole>().Any(x => x.RoleTemplateId == licenseAdminTemplateId);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            var roleAssignments = await graphClient.Users[userId].MemberOf.Request().GetAsync().ConfigureAwait(false);
            // var roles = await graphClient.DirectoryRoles.Request().GetAsync().ConfigureAwait(false);
            //var members = await graphClient.DirectoryRoles[licenseAdminId].Members.Request().GetAsync().ConfigureAwait(false);

            return roleAssignments.OfType<DirectoryRole>().Any(x => x.RoleTemplateId == role || x.DisplayName == role);
        }
    }
}
