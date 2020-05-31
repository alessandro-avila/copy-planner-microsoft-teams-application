using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TeamsAppLib.Common;
using TeamsAppLib.Log;
using TeamsAppLib.Messages;
using TeamsAppLib.Models;
using TeamsAppLib.Settings;

namespace TeamsAppLib.Api
{
    public class TeamApi : HttpClientApi
    {
        public TeamApi(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Get the teams in Microsoft Teams that the user is a direct member of.
        /// </summary>
        /// <returns>If successful, this method returns a 200 OK response code and collection of group objects in the response body.</returns>
        public async Task<Team[]> GetJoinedTeams()
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            Team[] teams;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + "/me/joinedTeams");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                var root = JsonConvert.DeserializeObject<RootElem<Team>>(httpResultString);
                teams = root.Values;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return teams;
        }

        /// <summary>
        /// Create a copy of a team. This operation also creates a copy of the corresponding group.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>If successful, this method will return a 202 Accepted response code with a Location: header pointing to the operation resource.
        /// When the operation is complete, the operation resource will tell you the id of the created team.</returns>
        public async Task<bool> CloneTeam(InCloneTeamMessage request)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            bool created = false;
            if (request == null)
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return created;
            }

            var bodyContent = JsonConvert.SerializeObject(request);
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.PostAsync(O365Settings.MsGraphBetaEndpoint + $"/teams/{request.TeamId}/clone",
                        new StringContent(bodyContent, Encoding.UTF8, "application/json"));
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                created = true;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }

            return created;
        }
    }
}
