using System.Collections.Concurrent;

namespace CurrencyConverter.Services
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RequestLog> _requestLogs = new();
        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress!.ToString();
            var now = DateTime.UtcNow;
            var requestLog = _requestLogs.GetOrAdd(ipAddress, new RequestLog());
            lock (requestLog)
            {
                requestLog.Requests.Enqueue(now);
                while (requestLog.Requests.Count > 0 && (now - requestLog.Requests.Peek()).TotalSeconds > 60)
                {
                    requestLog.Requests.Dequeue();
                }
                if (requestLog.Requests.Count > 10)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.WriteAsync("Rate limit exceeded");
                    return;
                }
            }
            await _next(context);
        }
    }
    public class RequestLog
    {
        public Queue<DateTime> Requests { get; } = new();
    }
}
