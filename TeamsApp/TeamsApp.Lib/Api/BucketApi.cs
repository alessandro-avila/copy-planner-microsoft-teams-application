using System;
using System.Net.Http;
using System.Threading.Tasks;
using TeamsAppLib.Common;
using TeamsAppLib.Enums;
using TeamsAppLib.Extensions;
using TeamsAppLib.Log;
using TeamsAppLib.Settings;

namespace TeamsAppLib.Api
{
    public class BucketApi : HttpClientApi
    {
        public BucketApi(string accessToken) : base(accessToken)
        {
        }

        /// <summary>
        /// Delete a plannerBucket.
        /// </summary>
        /// <param name="bucketId">The plannerBucket's ID to delete.</param>
        /// <param name="eTag">eTag of the plannerBucket resource.</param>
        /// <returns>If successful, this method returns 204 No Content response code. It does not return anything in the response body. In case of errors, see HTTP status codes.</returns>
        public async Task DeleteBucket(string bucketId, string eTag)
        {
            // C# 8.0 Preview 2 feature.
            using var cs = this.GetCodeSection();

            if (string.IsNullOrWhiteSpace(bucketId)
                || string.IsNullOrWhiteSpace(eTag))
            {
                cs.Warning(Constants.MESSAGE_WARNING_NULLARGUMENTS);
                return;
            }

            try
            {
                HttpResponseMessage httpResponseMessage = null;
                var retry = new RetryWithExponentialBackoff<HttpResponseMessage>();
                await retry.RunAsync(
                    async () =>
                    {
                        httpResponseMessage = await HttpClient.SendTeamsAsync(
                            HttpVerb.DELETE,
                            new Uri(O365Settings.MsGraphBetaEndpoint + $"/planner/buckets/{bucketId}"),
                            eTag,
                            null);
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
                throw ex;
            }
        }
    }
}
