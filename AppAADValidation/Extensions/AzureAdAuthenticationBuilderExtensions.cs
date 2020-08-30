using APIDoctores.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIDoctores.Extensions
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
     => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;
            private readonly IGraphAuthProvider _authProvider;

            public AzureAdOptions GetAzureAdOptions() => _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions, IGraphAuthProvider authProvider)
            {
                _azureOptions = azureOptions.Value;
                _authProvider = authProvider;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.ClientSecret = _azureOptions.ClientSecret;
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("offline_access");

                //options.Authority = $"{_authProvider.Authority}v2.0";
                //options.UseTokenLifetime = true;
                //options.CallbackPath = _azureOptions.CallbackPath;
                //options.RequireHttpsMetadata = false;
                //options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                //options.Scope.Add( $"{_azureOptions.ApiUrl}.default" );

                /*options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Ensure that User.Identity.Name is set correctly after login
                    NameClaimType = "name",

                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };*/

                /* options.Events = new OpenIdConnectEvents
                 {
                     OnTicketReceived = context =>
                     {
                         // If your authentication logic is based on users then add your logic here
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         context.Response.Redirect("/Home/Error");
                         context.HandleResponse(); // Suppress the exception
                         return Task.CompletedTask;
                     },
                     OnAuthorizationCodeReceived = async (context) =>
                     {
                         var code = context.ProtocolMessage.Code;
                         var identifier = context.Principal.FindFirst(Startup.ObjectIdentifierType).Value;

                         var result = await _authProvider.GetUserAccessTokenByAuthorizationCode(code);

                         // Check whether the login is from the MSA tenant. 
                         // The sample uses this attribute to disable UI buttons for unsupported operations when the user is logged in with an MSA account.
                         var currentTenantId = context.Principal.FindFirst(Startup.TenantIdType).Value;
                         if (currentTenantId == "08723d40-ab7f-4afa-8b96-0f1d67082448")
                         {
                             // MSA (Microsoft Account) is used to log in
                         }

                         context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                     },
                     // If your application needs to do authenticate single users, add your user validation below.
                     //OnTokenValidated = context =>
                     //{
                     //    return myUserValidationLogic(context.Ticket.Principal);
                     //}
                 };*/
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
