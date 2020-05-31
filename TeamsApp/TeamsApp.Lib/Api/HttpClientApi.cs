using System.Net.Http;
using System.Net.Http.Headers;
using TeamsAppLib.Interfaces;

namespace TeamsAppLib.Api
{
    public abstract class HttpClientApi : Api, IHttpClient
    {
        public HttpClient HttpClient { get; set; }

        public HttpClientApi(string accessToken)
        {
            _accessToken = accessToken;
            this.HttpClient = new HttpClient();
            this.SetHttpClient();
        }

        public void SetHttpClient()
        {
            this.HttpClient.DefaultRequestHeaders.Clear();
            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            this.HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
