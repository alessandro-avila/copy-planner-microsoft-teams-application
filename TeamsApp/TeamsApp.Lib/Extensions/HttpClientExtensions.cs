using System;
using System.Net.Http;
using System.Threading.Tasks;
using TeamsAppLib.Enums;

namespace TeamsAppLib.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> SendTeamsAsync(this HttpClient client, HttpVerb verb, Uri requestUri, string eTag, HttpContent iContent = null)
        {
            HttpMethod method;
            switch (verb)
            {
                case HttpVerb.PATCH:
                    method = new HttpMethod("PATCH");
                    break;
                case HttpVerb.DELETE:
                    method = new HttpMethod("DELETE");
                    break;
                default:
                    throw new ArgumentNullException();
            }

            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent,
            };
            request.Headers.TryAddWithoutValidation("If-Match", eTag);

            var response = new HttpResponseMessage();

            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException tce)
            {
                throw tce;
            }
            return response;
        }
    }
}
