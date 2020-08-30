using APIDoctores.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIDoctores.Services
{
    public class GraphAuthProvider : IGraphAuthProvider
    {
        private readonly IConfidentialClientApplication _app;
        private readonly string[] _scopes;
        private readonly AzureAdOptions _azureOptions;

        public GraphAuthProvider(IConfiguration configuration)
        {
            _azureOptions = new AzureAdOptions();
            configuration.Bind("AzureAd", _azureOptions);

            _app = ConfidentialClientApplicationBuilder.Create(_azureOptions.ClientId)
                    .WithClientSecret(_azureOptions.ClientSecret)
                    .WithAuthority(new Uri(_azureOptions.Authority))
                    .Build();

            Authority = _app.Authority;

            _scopes = new string[] { $"{_azureOptions.ApiUrl}.default" };
        }

        public string Authority { get; }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var result = await _app.AcquireTokenForClient(_scopes).ExecuteAsync();
                return result.AccessToken;
            }
            // Unable to retrieve the access token silently.
            catch (Exception)
            {
                throw new ServiceException(new Error
                {
                    Code = GraphErrorCode.AuthenticationFailure.ToString(),
                    Message = "Caller needs to authenticate. Unable to retrieve the access token silently."
                });
            }
        }

        // Gets an access token. First tries to get the access token from the token cache.
        // Using password (secret) to authenticate. Production apps should use a certificate.
        public async Task<string> GetUserAccessTokenAsync(string userId)
        {
            var account = await _app.GetAccountAsync(userId);
            if (account == null) throw new ServiceException(new Error
            {
                Code = "TokenNotFound",
                Message = "User not found in token cache. Maybe the server was restarted."
            });

            try
            {
                var result = await _app.AcquireTokenSilent(_scopes, account).ExecuteAsync();
                return result.AccessToken;
            }

            // Unable to retrieve the access token silently.
            catch (Exception)
            {
                throw new ServiceException(new Error
                {
                    Code = GraphErrorCode.AuthenticationFailure.ToString(),
                    Message = "Caller needs to authenticate. Unable to retrieve the access token silently."
                });
            }
        }

        public async Task<AuthenticationResult> GetUserAccessTokenByAuthorizationCode(string authorizationCode)
        {
            return await _app.AcquireTokenByAuthorizationCode(_scopes, authorizationCode).ExecuteAsync();
        }
    }

    public interface IGraphAuthProvider
    {
        string Authority { get; }

        Task<string> GetAccessTokenAsync();

        Task<string> GetUserAccessTokenAsync(string userId);

        Task<AuthenticationResult> GetUserAccessTokenByAuthorizationCode(string authorizationCode);
    }
}
