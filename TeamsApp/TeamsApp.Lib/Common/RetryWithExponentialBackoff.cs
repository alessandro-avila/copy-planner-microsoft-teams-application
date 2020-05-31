using System;
using System.Net.Http;
using System.Threading.Tasks;
using TeamsAppLib.Log;
using TeamsAppLib.Settings;

namespace TeamsAppLib.Common
{
    // See https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/explore-custom-http-call-retries-exponential-backoff for more information.
    public sealed class RetryWithExponentialBackoff<T>
       where T : HttpResponseMessage
    {
        private readonly int maxRetries;
        private readonly int delayMilliseconds;
        private readonly int maxDelayMilliseconds;

        public RetryWithExponentialBackoff(
            int maxRetries = Retry.MAXRETRIES,
            int delayMilliseconds = Retry.DELAYMILLISECONDS,
            int maxDelayMilliseconds = Retry.MAXDELAYMILLISECONDS
            )
        {
            this.maxRetries = maxRetries;
            this.delayMilliseconds = delayMilliseconds;
            this.maxDelayMilliseconds = maxDelayMilliseconds;
        }

        public async Task RunAsync(Func<Task<T>> func)
        {
            // C# 8.0 Preview 2 feature. 
            using var cs = this.GetCodeSection();

            var backoff = new ExponentialBackoff(
                this.maxRetries,
                this.delayMilliseconds,
                this.maxDelayMilliseconds);

            while (true)
            {
                try
                {
                    cs.Debug($"Calling: {func.Method}...");
                    T res = await func();
                    if (!res.IsSuccessStatusCode)
                    {
                        throw new Exception(res.ReasonPhrase);
                    }
                    cs.Debug($"{func.Method} successfully called.");
                    break;
                }
                catch (Exception ex)
                {
                    cs.Exception(ex);
                    cs.Debug("Retry.");

                    try
                    {
                        await backoff.Delay();
                    }
                    catch (Exception ex1)
                    {
                        cs.Exception(ex1);
                        throw;
                    }
                }
            }
        }
    }

    public class ExponentialBackoff
    {
        private readonly int maxRetries;
        private readonly int delayMilliseconds;
        private readonly int maxDelayMilliseconds;

        private int retries;
        private int pow;

        public ExponentialBackoff(
            int maxRetries,
            int delayMilliseconds,
            int maxDelayMilliseconds
            )
        {
            this.maxRetries = maxRetries;
            this.delayMilliseconds = delayMilliseconds;
            this.maxDelayMilliseconds = maxDelayMilliseconds;
            retries = 0;
            pow = 1;
        }

        public Task Delay()
        {
            if (retries == this.maxRetries)
            {
                throw new TimeoutException(Retry.ERROR_MAXRETRIESATTEMPTSEXCEEDED);
            }
            retries++;
            if (retries < 31)
            {
                pow <<= 1;
            }

            int delay = Math.Min(this.delayMilliseconds * (pow - 1) / 2, this.maxDelayMilliseconds);
            return Task.Delay(delay);
        }
    }
}
