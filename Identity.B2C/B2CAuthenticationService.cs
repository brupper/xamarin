using Brupper.Identity.B2C.Models;
using Microsoft.Identity.Client;
using MvvmCross;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace B2C
{
    /// <summary> For simplicity, we'll have this as a singleton.  </summary>
    public class B2CAuthenticationService
    {
        #region Singleton like...

        private static readonly Lazy<B2CAuthenticationService> lazy = new Lazy<B2CAuthenticationService>(() => new B2CAuthenticationService());

        public static B2CAuthenticationService Instance => lazy.Value;

        #endregion

        private readonly IB2CSettings settings;
        private readonly IPublicClientApplication publicClientApplication;

        private B2CAuthenticationService()
        {
            var settingsFound = Mvx.IoCProvider.TryResolve<IB2CSettings>(out var settings);
            this.settings = settings;

            // default redirectURI; each platform specific project will have to override it with its own
            var builder = PublicClientApplicationBuilder.Create(settings.ClientID)
                .WithB2CAuthority(settings.AuthoritySignInSignUp)
                .WithIosKeychainSecurityGroup(B2CConstants.IOSKeyChainGroup)
                .WithRedirectUri($"msal{settings.ClientID}://auth");

            // Android implementation is based on https://github.com/jamesmontemagno/CurrentActivityPlugin
            // iOS implementation would require to expose the current ViewControler - not currently implemented as it is not required
            // UWP does not require this

            Mvx.IoCProvider.TryResolve<IParentWindowLocatorService>(out var windowLocatorService);
            if (windowLocatorService != null)
            {
                builder = builder.WithParentActivityOrWindow(() => windowLocatorService?.GetCurrentParentWindow());
            }

            publicClientApplication = builder.Build();
        }

        public async Task<JwtUser> SignInAsync()
        {
            JwtUser newContext;
            try
            {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException)
            {
                // acquire token interactive
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        private async Task<JwtUser> AcquireTokenSilent()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();
            var authResult = await publicClientApplication.AcquireTokenSilent(settings.Scopes, GetAccountByPolicy(accounts, settings.PolicySignUpSignIn))
               .WithB2CAuthority(settings.AuthoritySignInSignUp)
               .ExecuteAsync();

            await RefreshUserDataAsync(authResult?.AccessToken).ConfigureAwait(false);
            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        private async Task<JwtUser> SignInInteractively()
        {
            //var accounts = await publicClientApplication.GetAccountsAsync();

            var authResult = await publicClientApplication.AcquireTokenInteractive(settings.Scopes)
                //.WithAccount(GetAccountByPolicy(accounts, settings.PolicySignUpSignIn))
                .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            await RefreshUserDataAsync(authResult?.AccessToken).ConfigureAwait(false);
            return newContext;
        }

        //public async Task<JwtUser> ResetPasswordAsync()
        //{
        //    var authResult = await publicClientApplication.AcquireTokenInteractive(settings.Scopes)
        //        .WithPrompt(Prompt.NoPrompt)
        //        .WithAuthority(settings.AuthorityPasswordReset)
        //        .ExecuteAsync();

        //    var userContext = UpdateUserInfo(authResult);

        //    return userContext;
        //}

        //public async Task<JwtUser> EditProfileAsync()
        //{
        //    var accounts = await publicClientApplication.GetAccountsAsync();

        //    var authResult = await publicClientApplication.AcquireTokenInteractive(settings.Scopes)
        //        .WithAccount(GetAccountByPolicy(accounts, settings.PolicyEditProfile))
        //        .WithPrompt(Prompt.NoPrompt)
        //        .WithAuthority(settings.AuthorityEditProfile)
        //        .ExecuteAsync();

        //    var userContext = UpdateUserInfo(authResult);

        //    return userContext;
        //}

        public async Task<JwtUser> SignOutAsync()
        {
            var accounts = await publicClientApplication.GetAccountsAsync();
            while (accounts.Any())
            {
                await publicClientApplication.RemoveAsync(accounts.FirstOrDefault());
                accounts = await publicClientApplication.GetAccountsAsync();
            }

            var signedOutContext = new JwtUser();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
        }

        private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            //Based on: https://damienaicheh.github.io/azure/xamarin/xamarin.forms/2019/07/01/sign-in-with-microsoft-account-with-xamarin-en.html
            return accounts?.FirstOrDefault();

            //foreach (var account in accounts)
            //{
            //    string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
            //    if (userIdentifier.EndsWith(policy.ToLower())) return account;
            //}

            //return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public JwtUser UpdateUserInfo(AuthenticationResult ar)
        {
            var joUser = ParseIdToken(ar.IdToken);
            var user = JsonConvert.DeserializeObject<JwtUser>(joUser.ToString());

            // user.AccessToken = ar.AccessToken; // always NULL
            user.IdToken = ar.IdToken;
            user.IsLoggedOn = true;

            return user;
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        /// <summary> Refresh user date from the Graph api. </summary>
        /// <param name="token">The user access token.</param>
        /// <returns>The current user with his associated informations.</returns>
        private async Task<JwtUser> RefreshUserDataAsync(string token)
        {
            var client = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Get, B2CConstants.GraphUrl);
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            var response = await client.SendAsync(message);
            JwtUser currentUser = null;

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                currentUser = JsonConvert.DeserializeObject<JwtUser>(json);
            }

            return currentUser;
        }
    }
}