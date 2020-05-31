using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamsAppLib.Helpers
{
    public sealed class AuthenticationHelper
    {
        private const string resourceId = "https://graph.microsoft.com/";
        private readonly string _clientId;
        private readonly string _redirectUri;
        private readonly string[] scopes =
            new[] {
            resourceId + "User.Read.All",
            resourceId + "Group.Read.All"
        };
        private string _accessToken;
        private DateTimeOffset _expiration;

        private GraphServiceClient graphClient = null;
        private PublicClientApplicationBuilder appBuilder;
        private AuthenticationResult _authResult = null;
        public AuthenticationResult AuthResult
        {
            get
            {
                if (_authResult == null)
                    return GetAccessTokenAsync().GetAwaiter().GetResult();
                return _authResult;
            }
        }

        private static AuthenticationHelper _instance;
        public static AuthenticationHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new NotImplementedException($"Error: not initialized instance.");
                }
                return _instance;
            }
        }

        private AuthenticationHelper(string clientId, string redirectUri)
        {
            this._clientId = clientId;
            this._redirectUri = redirectUri;
            this.appBuilder = PublicClientApplicationBuilder
                .Create(clientId)
                .WithRedirectUri(redirectUri);
        }

        public static void Init(string clientId, string redirectUri)
        {
            _instance = new AuthenticationHelper(clientId, redirectUri);
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync()
        {
            try
            {
                this._authResult = await this.appBuilder.Build().AcquireTokenInteractive(scopes).ExecuteAsync();
                _accessToken = this._authResult.AccessToken;
            }
            catch (Exception)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token.
                if (_accessToken == null || _expiration <= DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    this._authResult = await this.appBuilder.Build().AcquireTokenInteractive(scopes).ExecuteAsync();
                    _accessToken = this._authResult.AccessToken;
                    _expiration = this._authResult.ExpiresOn;
                }
            }
            return this._authResult;
        }

        public async void SignOut()
        {
            await appBuilder.Build().RemoveAsync(AuthResult.Account);
            _accessToken = null;
            graphClient = null;
        }

        public GraphServiceClient GetAuthenticatedClient()
        {
            graphClient = new GraphServiceClient(
              new DelegateAuthenticationProvider(async (requestMessage) =>
              {
                  requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _accessToken ?? (await GetAccessTokenAsync()).AccessToken);
              }));
            return graphClient;
        }
    }
}
