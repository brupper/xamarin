using Azure;
using Brupper.Jobs.FileTransfer.Azure.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Brupper.Jobs.FileTransfer.Azure;

/// <summary>
/// services.AddHttpClient(TokenKey, (sp, client) =>
///             {
///             var settings = sp.GetService<IClientAccessTokenRetriever>();
///             client.BaseAddress = new Uri(new Uri(settings.Endpoint), Constants.Endpoint_SasToken);
///                             client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
///             client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
///             
///             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"{JwtAccessToken}");
///             });
/// </summary>
public interface ITokenService
{
    Task<string> FetchSasTokenAsync();
    Task ResetAsync();
}

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class TokenService : ITokenService
{
    public const string TokenKey = "sastoken";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

    #region Constructor

    public TokenService(IHttpClientFactory httpClientFactory) { this.httpClientFactory = httpClientFactory; }

    #endregion

    public async Task ResetAsync()
    {
        cache.Remove(TokenKey);

        await Task.CompletedTask;
    }

    public async Task<string> FetchSasTokenAsync()
    {
        if (!cache.TryGetValue(TokenKey, out string token))
        {
            var tokenModel = await AcquireTokenAsync();
            /*
            if (tokenModel.Exception.ServerMessage != null || (tokenModel.Errors?.Any() ?? false))
            {
                throw new AccessTokenRetrieveException($"{tokenModel.Exception.Code} - {tokenModel.Exception.ServerMessage}") { Json = JsonConvert.SerializeObject(tokenModel) };
            }
            // */

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenModel.Value.Ttl) - TimeSpan.FromMinutes(5)); // lejarat elott 5 perccel...
            token = tokenModel.Value.Token;
            cache.Set(TokenKey, token, options);
        }

        return token;
    }

    private async Task<Response<SasResponse>> AcquireTokenAsync()
    {
        using (var client = httpClientFactory.CreateClient(TokenKey))
        {
            // Token request
            var response = await client.GetAsync("");
            if (!response.IsSuccessStatusCode)
            {
                throw new AccessTokenRetrieveException($"{response.StatusCode} - Server error");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response<SasResponse>>(json);
        }
    }
}
