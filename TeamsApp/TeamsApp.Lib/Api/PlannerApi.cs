using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
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
    public class PlannerApi : HttpClientApi
    {
        public PlannerApi(string accessToken)
            : base(accessToken)
        {
        }

        /// <summary>
        /// Retrieve a list of plannerPlan objects owned by a group object.
        /// </summary>
        /// <param name="groupId">The ID of the group (team) to retrieve the list of plannerPlan from.</param>
        /// <returns>If successful, this method returns a 200 OK response code and collection of plannerPlan objects in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerPlan[]> GetPlanners(string groupId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            PlannerPlan[] planners;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + $"/groups/{groupId}/planner/plans/");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                var root = JsonConvert.DeserializeObject<RootElem<PlannerPlan>>(httpResultString);
                planners = root.Values;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return planners;
        }

        /// <summary>
        /// Retrieve the properties and relationships of plannerplandetails object.
        /// </summary>
        /// <param name="planId">The plan's ID to get the properties and relationships from.</param>
        /// <returns>If successful, this method returns a 200 OK response code and plannerPlanDetails object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerPlanDetails> GetPlannerDetails(string planId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            PlannerPlanDetails details;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + $"/planner/plans/{planId}/details");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                details = JsonConvert.DeserializeObject<PlannerPlanDetails>(httpResultString);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return details;
        }

        /// <summary>
        /// Update the properties of plannerplandetails object.
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="eTag"></param>
        /// <param name="categoryDescriptions"></param>
        /// <param name="sharedWith"></param>
        /// <returns>If successful, this method returns a 200 OK response code and updated plannerPlanDetails object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task UpdatePlannerDetails(string planId, string eTag, object categoryDescriptions, object sharedWith)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId)
                || string.IsNullOrWhiteSpace(eTag)
                || categoryDescriptions == null
                || sharedWith == null)
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            dynamic body = new ExpandoObject();
            body.categoryDescriptions = categoryDescriptions;
            body.sharedWith = sharedWith;
            var bodyContent = JsonConvert.SerializeObject(body);

            cs.Debug($"Body content:\n-Plan Id: {planId}\n-Category Description: {body.categoryDescriptions}\n-Shared With: {body.sharedWith}");
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.SendTeamsAsync
                        (
                            HttpVerb.PATCH,
                            new Uri(O365Settings.MsGraphBetaEndpoint + $"/planner/plans/{planId}/details"),
                            eTag,
                            new StringContent(bodyContent, Encoding.UTF8, "application/json")
                        );
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

        /// <summary>
        /// Retrieve a list of plannerBucket objects contained by a PlannerPlan object.
        /// </summary>
        /// <param name="planId">The plannerplan's ID to get the list of plannerBucket from.</param>
        /// <returns>If successful, this method returns a 200 OK response code and collection of plannerBucket objects in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerBucket[]> GetBuckets(string planId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            PlannerBucket[] buckets;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + $"/planner/plans/{planId}/buckets");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                var root = JsonConvert.DeserializeObject<RootElem<PlannerBucket>>(httpResultString);
                buckets = root.Values;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return buckets;
        }

        /// <summary>
        /// Retrieve a list of plannertask objects associated to a plannerPlan object.
        /// </summary>
        /// <param name="planId">The plannerPlan's ID to get the list of plannertask from.</param>
        /// <returns>If successful, this method returns a 200 OK response code and collection of plannerTask objects in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerTask[]> GetTasks(string planId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            PlannerTask[] tasks;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + $"/planner/plans/{planId}/tasks");
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                var root = JsonConvert.DeserializeObject<RootElem<PlannerTask>>(httpResultString);
                tasks = root.Values;
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return tasks;
        }

        /// <summary>
        /// This method allows to create buckets and tasks in the recipient planner object.
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="sBuckets"></param>
        /// <param name="sTasks"></param>
        /// <returns>If successful, this method returns a Task object, an exception otherwise.</returns>
        public async Task CreateBucketsAndTasks(string planId, PlannerBucket[] sBuckets, PlannerTask[] sTasks)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId)
                || sBuckets == null)
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            string lastBucketOrderHint = "";
            foreach (var bucket in sBuckets)
            {
                var newBucket = await CreateBucket(planId, lastBucketOrderHint, bucket);
                lastBucketOrderHint = newBucket.OrderHint;

                try
                {
                    // Get bucket's tasks and sort in descending order by OrderHint.
                    var bTasks = sTasks.ToList().Where(t => t.BucketId == bucket.Id).OrderByDescending(t => t.OrderHint);
                    foreach (var task in bTasks)
                    {
                        var newTask = await CreateTask(planId, newBucket, task);

                        var taskApi = new TaskApi(_accessToken);
                        // Get old task details.
                        var oldTaskDetails = await taskApi.GetTaskDetails(task.Id);
                        if (oldTaskDetails == null)
                        {
                            throw new Exception($"GetTaskDetails returns null for task: {task}.");
                        }
                        // Get new task details.
                        var newTaskDetails = await taskApi.GetTaskDetails(newTask.Id);
                        if (newTaskDetails == null)
                        {
                            throw new Exception($"GetTaskDetails returns null for task: {newTask}.");
                        }

                        await taskApi.UpdateTaskDetails(newTask.Id,
                            newTaskDetails.ETag,
                            oldTaskDetails.Checklist,
                            oldTaskDetails.Description,
                            oldTaskDetails.PreviewType,
                            oldTaskDetails.References);
                    }
                }
                catch (Exception ex)
                {
                    cs.Exception(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Use this API to create a new plannerBucket.
        /// </summary>
        /// <param name="planId">The plannerplan's ID to create a new plannerbucket into.</param>
        /// <param name="lastBucketOrderHint"></param>
        /// <param name="sBucket"></param>
        /// <returns>If successful, this method returns 201 Created response code and plannerBucket object in the response body. In case of errors, see HTTP status codes.</returns>
        private async Task<PlannerBucket> CreateBucket(string planId, string lastBucketOrderHint, PlannerBucket sBucket)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            dynamic body = new ExpandoObject();
            body.name = sBucket.Name;
            body.planId = planId;
            body.orderHint = lastBucketOrderHint + " !";
            var bodyContent = JsonConvert.SerializeObject(body);

            cs.Debug($"\nBody content:\n{body.name}\n{body.planId}\n{body.orderHint}");

            PlannerBucket newBucket;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.PostAsync(
                        new Uri(O365Settings.MsGraphBetaEndpoint + $"/planner/buckets"),
                        new StringContent(bodyContent, Encoding.UTF8, "application/json"));
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                newBucket = JsonConvert.DeserializeObject<PlannerBucket>(httpResultString);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return newBucket;
        }

        /// <summary>
        /// Use this API to create a new plannerTask.
        /// </summary>
        /// <param name="planId">The plannerplan's ID to create a new plannerTask into.</param>
        /// <param name="bucket"></param>
        /// <param name="sTask"></param>
        /// <returns>If successful, this method returns 201 Created response code and plannerTask object in the response body. In case of errors, see HTTP status codes.</returns>
        private async Task<PlannerTask> CreateTask(string planId, PlannerBucket bucket, PlannerTask sTask)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(planId)
                || bucket == null
                || sTask == null)
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            dynamic body = new ExpandoObject();
            body.planId = planId;
            body.bucketId = bucket.Id;
            body.title = sTask.Title;
            body.startDateTime = sTask.StartDateTime;
            body.dueDateTime = sTask.DueDateTime;
            var bodyContent = JsonConvert.SerializeObject(body);

            cs.Debug($"\nBody content:\n-Title: {body.title}\n-Plan Id: {body.planId}");

            PlannerTask newTask;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.PostAsync(
                                                new Uri(O365Settings.MsGraphBetaEndpoint + $"/planner/tasks"),
                                                new StringContent(bodyContent, Encoding.UTF8, "application/json"));
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                newTask = JsonConvert.DeserializeObject<PlannerTask>(httpResultString);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return newTask;
        }

        /// <summary>
        /// Retrieve the properties and relationships of plannerplan object.
        /// </summary>
        /// <param name="plannerId">The plannerplan's ID to get the properties and relationships from.</param>
        /// <returns>If successful, this method returns a 200 OK response code and plannerPlan object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task<PlannerPlan> GetPlanner(string plannerId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(plannerId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return null;
            }

            PlannerPlan plannerPlan;
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.GetAsync(O365Settings.MsGraphBetaEndpoint + "planner/plans/" + plannerId);
                        return httpResponseMessage;
                    });

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var ex = new HttpRequestException(Constants.EXCEPTION_HTTPREQUEST + $" Status Code: {httpResponseMessage.StatusCode}.");
                    cs.Exception(ex);
                    throw ex;
                }
                var httpResultString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                plannerPlan = JsonConvert.DeserializeObject<PlannerPlan>(httpResultString);
            }
            catch (Exception ex)
            {
                cs.Exception(ex);
                throw;
            }
            return plannerPlan;
        }

        /// <summary>
        /// Use this API to create a new plannerPlan.
        /// </summary>
        /// <param name="groupId">The group's ID (team) to create the plannerPlan into.</param>
        /// <returns>If successful, this method returns 201 Created response code and plannerPlan object in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task CreatePlannerPlan(string groupId)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(groupId))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            var plannerPlan = new PlannerPlan()
            {
                Id = groupId
            };
            var plannerPlanContent = JsonConvert.SerializeObject(plannerPlan);
            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.PostAsync(O365Settings.MsGraphBetaEndpoint + "/planner/plans",
                        new StringContent(plannerPlanContent, Encoding.UTF8, "application/json"));
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
