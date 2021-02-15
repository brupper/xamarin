using Brupper.Data.RemoteServices.Exceptions;
using Brupper.Data.RemoteServices.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Brupper.Data.RemoteServices
{
    public abstract class ARemoteService<TApiModel, TModel, TDto> : ARemoteService, IRemoteService
        where TApiModel : Response<TDto>, new()
    {
        protected ARemoteService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
    }

    public abstract class ARemoteService : IRemoteService
    {
        public IHttpClientFactory Factory { get; }

        #region Constructor

        protected ARemoteService(
             IHttpClientFactory httpClientFactory)
        {
            Factory = httpClientFactory;
        }

        #endregion

        public abstract Task<StatusResponse> GetApiStatusAsync(CancellationToken cancellationToken = default);

        #region Helpers

        protected virtual async Task<HttpResponseMessage> ExecuteAndRefreshTokenIfRequiredAsync(
            Func<HttpClient> createHttpClient,
            Func<HttpClient, Task<HttpResponseMessage>> taskToExecute,
            CancellationToken token,
            int tryCounter = 3)
        {
            if (taskToExecute == null)
            {
                throw new ApiException(HttpErrorLabels.ErrorCode_ArgumentNull);
            }

            token.ThrowIfCancellationRequested();
            try
            {
                var client = createHttpClient(); // after refreshing an access token we have to recreate the client (for e.g.)
                var response = await taskToExecute(client).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                var tokenRefreshed = false;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    tokenRefreshed = await RefreshTokenAsync(token);
                    // nincs refresh token szoval fallback user ujra bejelentkezik...
                }

                if (tokenRefreshed)
                {
                    client = createHttpClient();    // recreate client to use new access token
                    response = await taskToExecute(client).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized && !tokenRefreshed)
                {
                    HandleUnauthorized();

                    // failed to refresh so return original 401 response
                    return response;
                }

                if ((int)response.StatusCode >= 500)
                {
                    await LogServerErrorAsync(response);
                }

                // HTTP 404 and other failed requests should not be stuck in endless cycle:
                if (tryCounter == 0)
                {
                    return response;
                }
            }
            catch (Exception exception)
            {
                // Crashes.TrackError(exception);

                if (tryCounter == 0)
                {
                    // too many tries, still exception happen
                    throw new ApiException(HttpErrorLabels.ErrorCode_TooManyAttemps, exception);
                }
            }

            return await ExecuteAndRefreshTokenIfRequiredAsync(createHttpClient, taskToExecute, token, tryCounter - 1);
        }

        protected virtual Task<bool> RefreshTokenAsync(CancellationToken cancellationToken) => Task.FromResult(false);

        protected async Task LogServerErrorAsync(HttpResponseMessage message)
        {
            if (message == null)
            {
                return;
            }

            try
            {
                var content = await message.Content.ReadAsStringAsync();

                var parameters = new Dictionary<string, string>
                {
                    { "RequestUrl", message.RequestMessage.RequestUri.ToString() },
                    { "Content", content },
                    { "Code", message.StatusCode.ToString() },
                };

                //TODO: Analytics.TrackEvent("Server error occured", parameters);
            }
            catch { /*ignore*/ }
        }

        protected abstract void HandleUnauthorized();

        protected virtual void ThrowIfRequired(HttpResponseMessage response, string data)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    break;

                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.InternalServerError:
                    throw new ApiException(data);
                default:
                    break;
            }
        }

        #endregion
    }
}
