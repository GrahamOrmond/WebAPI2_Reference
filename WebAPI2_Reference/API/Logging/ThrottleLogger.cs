using System.Web.Http.Tracing;
using WebApiThrottle;

namespace WebAPI2_Reference.API.Logging
{
    public class ThrottleLogger : IThrottleLogger
    {
        private readonly ITraceWriter traceWriter;

        public ThrottleLogger(ITraceWriter traceWriter)
        {
            this.traceWriter = traceWriter;
        }

        public void Log(ThrottleLogEntry entry)
        {
            if (null != traceWriter)
            {
                traceWriter.Info(entry.Request, "WebApiThrottle",
                    "{0} Request {1} from {2} has been throttled (blocked), quota {3}/{4} exceeded by {5}",
                    entry.LogDate, entry.RequestId, entry.ClientIp, entry.RateLimit, entry.RateLimitPeriod, entry.TotalRequests);
            }
        }
    }
}
