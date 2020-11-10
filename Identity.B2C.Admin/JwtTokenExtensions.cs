using B2C;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Brupper.Identity.B2C
{
    public static class JwtTokenExtensions
    {
        private static (DateTime stamp, OpenIdConnectConfiguration config) tokenCache = (DateTime.MinValue, null);

        public static string ParseToken(HttpRequest request)
        {
            var header = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
            var authHeader = header.Parameter;

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                //#if DEBUG
                //return authHeader;
                //#endif
                var s = encoding.GetString(Convert.FromBase64String(authHeader));
                if (!Guid.TryParse(s, out _))
                    return null;

                return s;
            }

            return null;
        }

        //public const string tokenValid = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJleHAiOjE1OTA1NjM1ODMsIm5iZiI6MTU5MDU1OTk4MywidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9wZmtiMmMuYjJjbG9naW4uY29tLzI2YTEwODlhLTBiOTMtNDFiYS1hM2ExLTVkNjAzODJkN2E4NS92Mi4wLyIsInN1YiI6ImYzODQ1ZjA4LTVlN2EtNGE0ZS05Yjc0LTU2ZGE0YjE0NGYyZiIsImF1ZCI6ImE1ZjUyMmI4LWIwZDQtNDVkOC05ZGEyLWNiZWJkNDU5MDYyNCIsImlhdCI6MTU5MDU1OTk4MywiYXV0aF90aW1lIjoxNTkwNTU5OTgzLCJpZHBfYWNjZXNzX3Rva2VuIjoiRXdCZ0E4bDZCQUFVTzljaGg4Y0pzY1FMbVUrTFNXcGJucjB2bXd3QUFjZ3VvZE0rWHV3RFpNbWdpeGQ0TURNc2xBMHBsV3hXOFFSeGdLd0FmMmkxVVlhb3ROMHJ2c245Kzc1dEtySDQ3M0E5VjNEOVdrVUhlajU5cnN2cWpKc1dzZnBGVzBjNU03YzVtbmU0Q28zMU9LUjJBdGlUZ010MmlNQVMwb3RxRXRXUnQrR0h4RXVnbEMvWStXL0UvUzUyWmd4VXAxTVZKdW5td3dJdG9BN1lOTnM5MnpwcUxuUFNyUng0QkV0RFB2aUdLS1p3MWc1WU1QdnVwKzNxVkI3Um9LY0ZDQzQxVk5vUzB0Z3FrSW1yR3dqSy9OaHNNcmhPMThvTFYyMkNUSWNORWN1SHo2WnBtcElwVHNvZ0tSd0ZsS3BscHZoMVIzTEhwdTd0b0ZtcVVLNGpTZG5lN05uMWF4RnNGNFJxaDNIQ0d5aUY5aDhxV21ia2JZck1jbUlEWmdBQUNHUEQvR2gzMmlhS01BSUhGa3dRZnpxSXRUNUlYSEpMUmp5SUVlYS9jejRmL205S0JYdGgyTnhnYStyaEZWZ29PdjJWZjhlQjBjYnNEM1BtVGh6aFgxd01oWERyZTJhSkcvaHZRcmt3QWVuVUIzbGtIbXFOWVNUQkhjeGxxTnNhcjNiNXN5ZlZ6SStQL0tSWlErLzEwV0tjUi9FbEJyc2R3Z3ZqVWJQQ0RIa1F5N2Q3dkwzSUVSazhOWVdTbDRYTmZvNlFzUmFpVnNEYXVTdEhZNG44a0doNWtBZUlLYWQyeDJKU1o3QWxqYy8xN2xVTUpkeTNMcU5EbUVuWjVYOWkwMWhvYmJJOHVKWHdvdVowcjlxL3FyUUxCaml6eVREd2tFOU9ETEc1YTZxSjdtc1NMTFFFbU9LbUNsU1NmQ3REc2JaZUFmU1FUVmoyWnFBVHc1a1YzNUM3MllkSnNQNFRVd3JPOEhmaXhqV25iOW9ZRzlBUElFZTdjdyt1dVBjZ3Bxc2dGUDVDWStpK3M1UkZLYW8yQUoraHorMFF4MDdUN3ZnN0xNdWw5TG13OUlwWGJ1ZFZlc21wRmQrZ0E2UnhuZXBoYjdDM0Nkc1pIQ29yZldvcUNwb1dlRTNYQzJWMER4K0JoeWIrZ0h5WjllK3B3S090U1Jlam5hVHY2UklxT2ZCL2pFcUFwRTk0RmN1VFNOSkFoUlJiSWhuWlY2eHZLc09mclRoVm01QUFRUmFaSTczV3JRWGpxK1dyK0thRXBrVERKd05UVzllU0YxWkptZzgvc3R5WDBGUTZ2aHFnWGxyUGI2TW5BbjREZUlOQVluQ0RRbnZJMHM0RE1qYWRxeTlMekc2K0kwWEEwRitrU0R0TC91aHhoZjZUWGVXZjRvSXA3S2hiUTF2NytuU2orbk5WSDlJNGNkd0NJVFo2am1tYjRpenIrTmJ3UWlIL21RS21rRlA0TGZNTnUwLzljeC9UV0RnblNuYUJoeXhMcVNLbGxyTFZLbElDIiwiaWRwIjoibGl2ZS5jb20iLCJvaWQiOiJmMzg0NWYwOC01ZTdhLTRhNGUtOWI3NC01NmRhNGIxNDRmMmYiLCJjaXR5IjoiSsOhbm9zaGlkYSIsImNvdW50cnkiOiJIdW5nYXJ5IiwiZ2l2ZW5fbmFtZSI6IkFkYW0iLCJwb3N0YWxDb2RlIjoiNTE0MyIsInN0YXRlIjoiSmFzei1OYWd5a3VuLVN6b2xub2siLCJzdHJlZXRBZGRyZXNzIjoiUGV0xZFmaSBTw6FuZG9yIDMiLCJmYW1pbHlfbmFtZSI6IkJhcsOhdGgiLCJlbWFpbHMiOlsiYWRhbWJhcmF0aEBtc24uY29tIl0sInRmcCI6IkIyQ18xX3Rlc3QifQ.XTweuES_JSq5egAVvebbd0gDvsGfhQBubsSf6AEyqxHFoW50NFfXim7z4m95dOtDtWfHNjYfjQWkj8MrRJVCpg30q9VQGwBCKoarjMht50_I3222MvmpzhVo5c3zRHoPa5OcIXvtr9wQQMvzpSsjGSzZr62teJkaR-5HavievTfMNcKZhJmdw6Bq9tFalvpHJNd1AUV36o9stY0dgF0QcjYLFC2nVaTCS-4EY2vBCaFF2COPtmBVH5ph4Ud2Muz3HU02R2KmqMbnpak8GzRB26QBpWTYOfLjXRanWo8wP688FsC8Qcss5YvKbALi0wZMSDwIuyGAalX9fwZIpDl5Hw";
        //public const string tokenManipulated = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.ewogICJleHAiOiAxNTkwNTYzNTgzLAogICJuYmYiOiAxNTkwNTU5OTgzLAogICJ2ZXIiOiAiMS4wIiwKICAiaXNzIjogImh0dHBzOi8vcGZrYjJjLmIyY2xvZ2luLmNvbS8yNmExMDg5YS0wYjkzLTQxYmEtYTNhMS01ZDYwMzgyZDdhODUvdjIuMC8iLAogICJzdWIiOiAiZjM4NDVmMDgtNWU3YS00YTRlLTliNzQtNTZkYTRiMTQ0ZjJmIiwKICAiYXVkIjogImE1ZjUyMmI4LWIwZDQtNDVkOC05ZGEyLWNiZWJkNDU5MDYyNCIsCiAgImlhdCI6IDE1OTA1NTk5ODMsCiAgImF1dGhfdGltZSI6IDE1OTA1NTk5ODMsCiAgImlkcF9hY2Nlc3NfdG9rZW4iOiAiRXdCZ0E4bDZCQUFVTzljaGg4Y0pzY1FMbVUrTFNXcGJucjB2bXd3QUFjZ3VvZE0rWHV3RFpNbWdpeGQ0TURNc2xBMHBsV3hXOFFSeGdLd0FmMmkxVVlhb3ROMHJ2c245Kzc1dEtySDQ3M0E5VjNEOVdrVUhlajU5cnN2cWpKc1dzZnBGVzBjNU03YzVtbmU0Q28zMU9LUjJBdGlUZ010MmlNQVMwb3RxRXRXUnQrR0h4RXVnbEMvWStXL0UvUzUyWmd4VXAxTVZKdW5td3dJdG9BN1lOTnM5MnpwcUxuUFNyUng0QkV0RFB2aUdLS1p3MWc1WU1QdnVwKzNxVkI3Um9LY0ZDQzQxVk5vUzB0Z3FrSW1yR3dqSy9OaHNNcmhPMThvTFYyMkNUSWNORWN1SHo2WnBtcElwVHNvZ0tSd0ZsS3BscHZoMVIzTEhwdTd0b0ZtcVVLNGpTZG5lN05uMWF4RnNGNFJxaDNIQ0d5aUY5aDhxV21ia2JZck1jbUlEWmdBQUNHUEQvR2gzMmlhS01BSUhGa3dRZnpxSXRUNUlYSEpMUmp5SUVlYS9jejRmL205S0JYdGgyTnhnYStyaEZWZ29PdjJWZjhlQjBjYnNEM1BtVGh6aFgxd01oWERyZTJhSkcvaHZRcmt3QWVuVUIzbGtIbXFOWVNUQkhjeGxxTnNhcjNiNXN5ZlZ6SStQL0tSWlErLzEwV0tjUi9FbEJyc2R3Z3ZqVWJQQ0RIa1F5N2Q3dkwzSUVSazhOWVdTbDRYTmZvNlFzUmFpVnNEYXVTdEhZNG44a0doNWtBZUlLYWQyeDJKU1o3QWxqYy8xN2xVTUpkeTNMcU5EbUVuWjVYOWkwMWhvYmJJOHVKWHdvdVowcjlxL3FyUUxCaml6eVREd2tFOU9ETEc1YTZxSjdtc1NMTFFFbU9LbUNsU1NmQ3REc2JaZUFmU1FUVmoyWnFBVHc1a1YzNUM3MllkSnNQNFRVd3JPOEhmaXhqV25iOW9ZRzlBUElFZTdjdyt1dVBjZ3Bxc2dGUDVDWStpK3M1UkZLYW8yQUoraHorMFF4MDdUN3ZnN0xNdWw5TG13OUlwWGJ1ZFZlc21wRmQrZ0E2UnhuZXBoYjdDM0Nkc1pIQ29yZldvcUNwb1dlRTNYQzJWMER4K0JoeWIrZ0h5WjllK3B3S090U1Jlam5hVHY2UklxT2ZCL2pFcUFwRTk0RmN1VFNOSkFoUlJiSWhuWlY2eHZLc09mclRoVm01QUFRUmFaSTczV3JRWGpxK1dyK0thRXBrVERKd05UVzllU0YxWkptZzgvc3R5WDBGUTZ2aHFnWGxyUGI2TW5BbjREZUlOQVluQ0RRbnZJMHM0RE1qYWRxeTlMekc2K0kwWEEwRitrU0R0TC91aHhoZjZUWGVXZjRvSXA3S2hiUTF2NytuU2orbk5WSDlJNGNkd0NJVFo2am1tYjRpenIrTmJ3UWlIL21RS21rRlA0TGZNTnUwLzljeC9UV0RnblNuYUJoeXhMcVNLbGxyTFZLbElDIiwKICAiaWRwIjogImxpdmUuY29tIiwKICAib2lkIjogImYzODQ1ZjA4LTVlN2EtNGE0ZS05Yjc0LTU2ZGE0YjE0NGYyZiIsCiAgImNpdHkiOiAiSsOhbm9zaGlkYSIsCiAgImNvdW50cnkiOiAiSHVuZ2FyeSIsCiAgImdpdmVuX25hbWUiOiAiQWRhbWthIiwKICAicG9zdGFsQ29kZSI6ICI1MTQzIiwKICAic3RhdGUiOiAiSmFzei1OYWd5a3VuLVN6b2xub2siLAogICJzdHJlZXRBZGRyZXNzIjogIlBldMWRZmkgU8OhbmRvciAzIiwKICAiZmFtaWx5X25hbWUiOiAiQmFyw6F0aCIsCiAgImVtYWlscyI6IFsKICAgICJhZGFtYmFyYXRoQG1zbi5jb20iCiAgXSwKICAidGZwIjogIkIyQ18xX3Rlc3QiCn0=.XTweuES_JSq5egAVvebbd0gDvsGfhQBubsSf6AEyqxHFoW50NFfXim7z4m95dOtDtWfHNjYfjQWkj8MrRJVCpg30q9VQGwBCKoarjMht50_I3222MvmpzhVo5c3zRHoPa5OcIXvtr9wQQMvzpSsjGSzZr62teJkaR-5HavievTfMNcKZhJmdw6Bq9tFalvpHJNd1AUV36o9stY0dgF0QcjYLFC2nVaTCS-4EY2vBCaFF2COPtmBVH5ph4Ud2Muz3HU02R2KmqMbnpak8GzRB26QBpWTYOfLjXRanWo8wP688FsC8Qcss5YvKbALi0wZMSDwIuyGAalX9fwZIpDl5Hw";

        /// <summary> https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview#validate-signature </summary>
        public static async Task<JwtSecurityToken> ValidateAsync(string token, IB2CSettings settings)
        {
            if (tokenCache.stamp < DateTime.UtcNow.AddDays(-1) || tokenCache.config == null)
            {

                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(settings.StsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
                tokenCache.config = await configManager.GetConfigurationAsync();
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = tokenCache.config.SigningKeys,
                RequireSignedTokens = false,
            };

            var tokendHandler = new JwtSecurityTokenHandler();

            var result = tokendHandler.ValidateToken(token, validationParameters, out var jwt);
            return jwt as JwtSecurityToken;
        }

        public static Task<JwtSecurityToken> ValidateAsync(this HttpRequest req, IB2CSettings settings)
        {
            req.Headers.TryGetValue("Authorization", out var authHeader);
            var token = authHeader.ToString().Replace("Bearer ", "").Replace("bearer ", "");
            return ValidateAsync(token, settings);
        }

        public static string GetUniqueId(this JwtSecurityToken securityToken) => securityToken?.Claims?.FirstOrDefault(x => x.Type == B2CJwtRegisteredClaimNames.Oid)?.Value;

        public static string GetEmail(this JwtSecurityToken securityToken) => securityToken?.Claims?.FirstOrDefault(x => x.Type == "emails")?.Value; //TODO: B2CJwtRegisteredClaimNames.Emails

        public static string GetMail(this Microsoft.Graph.User user)
            => user?.Identities?.FirstOrDefault(x => x.SignInType == "emailAddress")?.IssuerAssignedId + " " + user?.Mail + "" + string.Join(", ", user?.OtherMails);
    }
}
