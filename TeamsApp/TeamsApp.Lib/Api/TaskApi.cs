using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TeamsAppLib.Common;
using TeamsAppLib.Enums;
using TeamsAppLib.Extensions;
using TeamsAppLib.Log;
using TeamsAppLib.Models;
using TeamsAppLib.Settings;

namespace TeamsAppLib.Api
{
    public class TaskApi : HttpClientApi
    {
        public TaskApi(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Retrieve the properties and relationships of plannertaskdetails object.
        /// </summary>
        /// <param name="taskId">The ID of the task whose properties and relationship are retreived.</param>
        /// <returns>If successful, this method returns a 200 OK response code and plannerTaskDetails object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerTaskDetails> GetTaskDetails(string taskId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(taskId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            PlannerTaskDetails details;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + $"/planner/tasks/{taskId}/details");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                details = JsonConvert.DeserializeObject<PlannerTaskDetails>(httpResultString);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return details;
        }

        /// <summary>
        /// Update the properties of plannertaskdetails object.
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="eTag"></param>
        /// <param name="checklist"></param>
        /// <param name="description"></param>
        /// <param name="previewType"></param>
        /// <param name="references"></param>
        /// <returns>If successful, this method returns a 200 OK response code and updated plannerTaskDetails object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task UpdateTaskDetails(string taskId, string eTag, Dictionary<string, Checklist> checklist = null, string description = null, string previewType = null, object references = null)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(taskId)
                || string.IsNullOrWhiteSpace(eTag))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            dynamic body = new ExpandoObject();
            if (checklist?.Count > 0)
            {
                var newChecklist = new Dictionary<string, object>();
                foreach (string k in checklist.Keys)
                {
                    var ch = new Dictionary<string, string>
                    {
                        { "@odata.type", "#microsoft.graph.plannerChecklistItem" },
                        { "title", checklist[k].Title },
                        { "isChecked", checklist[k].IsChecked.ToString() }
                    };
                    newChecklist.Add(Guid.NewGuid().ToString(), ch);
                }
                body.checklist = newChecklist;
            }
            body.description = description;
            body.previewType = previewType;
            body.references = references;
            // TODO: use JsonConverter.
            var bodyContent = JsonConvert.SerializeObject(body);

            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.SendTeamsAsync(
                            HttpVerb.PATCH,
                            new Uri(O365Settings.MsGraphBetaEndpoint + $"/planner/tasks/{taskId}/details"),
                            eTag,
                            new StringContent(bodyContent, Encoding.UTF8, "application/json"));
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
        }
    }
}
