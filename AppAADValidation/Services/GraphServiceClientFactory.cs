using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace APIDoctores.Services
{
    public class GraphServiceClientFactory : IGraphServiceClientFactory
    {
        public IGraphAuthProvider _authProvider;

        public GraphServiceClientFactory(IGraphAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        public GraphServiceClient GetAuthenticatedGraphClient() {
            return new GraphServiceClient(new DelegateAuthenticationProvider(
                 async requestMessage =>
                 {
                     var accessToken = await _authProvider.GetAccessTokenAsync();

                    // Append the access token to the request
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                 }));
          }
    }

    public interface IGraphServiceClientFactory
    {
        GraphServiceClient GetAuthenticatedGraphClient();
    }
}
