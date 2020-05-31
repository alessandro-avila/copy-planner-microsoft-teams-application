using System.Net.Http;

namespace TeamsAppLib.Interfaces
{
    internal interface IHttpClient
    {
        HttpClient HttpClient { get; set; }
        void SetHttpClient();
    }
}
