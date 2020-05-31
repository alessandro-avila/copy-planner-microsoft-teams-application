using CacheManager.Core;
using TeamsAppLib.Api;
using TApi = TeamsAppLib.Api.Api;

namespace TeamsAppLib.Factory
{
    public static class TeamsFactory
    {
        private static ICacheManager<TApi> _cacheManager
            = CacheFactory.Build<TApi>("teamsCache", settings => settings.WithSystemRuntimeCacheHandle("handleName"));

        public static TApi GetStaticApi(string key, string accessToken)
        {
            TApi apiClass = null;
            switch (key)
            {
                case "TeamApi":
                    if (_cacheManager.Get<TApi>(key) != null)
                    {
                        return _cacheManager.Get<TApi>(key);
                    }
                    else
                    {
                        apiClass = new TeamApi(accessToken);
                    }
                    break;
                case "BucketApi":
                    if (_cacheManager.Get<TApi>(key) != null)
                    {
                        return _cacheManager.Get<TApi>(key);
                    }
                    else
                    {
                        apiClass = new BucketApi(accessToken);
                    }
                    break;
                case "ChannelApi":
                    if (_cacheManager.Get<TApi>(key) != null)
                    {
                        return _cacheManager.Get<TApi>(key);
                    }
                    else
                    {
                        apiClass = new ChannelApi(accessToken);
                    }
                    break;
                case "PlannerApi":
                    if (_cacheManager.Get<TApi>(key) != null)
                    {
                        return _cacheManager.Get<TApi>(key);
                    }
                    else
                    {
                        apiClass = new PlannerApi(accessToken);
                    }
                    break;
                case "TaskApi":
                    if (_cacheManager.Get<TApi>(key) != null)
                    {
                        return _cacheManager.Get<TApi>(key);
                    }
                    else
                    {
                        apiClass = new TaskApi(accessToken);
                    }
                    break;
                default:
                    break;
            }
            _cacheManager.Add(key, apiClass);
            return apiClass;
        }
    }
}
