using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TeamsAppLib.Common;
using TeamsAppLib.Log;
using TeamsAppLib.Models;
using TeamsAppLib.Settings;

namespace TeamsAppLib.Api
{
    public class ChannelApi : HttpClientApi
    {
        public ChannelApi(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Retrieve the properties and relationships of a channel.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns>If successful, this method returns a 200 OK response code and a channel object in the response body.</returns>
        public async Task<Channel[]> GetChannels(string teamId)
        {
            // C# 8.0 Preview 2 feature.
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(teamId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            Channel[] channels = null;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + "teams/" + teamId + "/channels");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                var root = JsonConvert.DeserializeObject<RootElem<Channel>>(httpResultString);
                channels = root.Values;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return channels;
        }
    }
}
