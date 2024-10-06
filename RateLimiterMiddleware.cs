using System.Diagnostics;

namespace SimpleRateLimiter;

/// <summary>
/// A simple rate limiter middleware.
/// </summary>
/// <param name="next">RequestDelegate</param>
/// <param name="bucketAlgorithmService">Algorithm which handles the logic to determine when a user will be allowed to process the request.</param>
public class RateLimiterMiddleware(RequestDelegate next, BucketAlgorithmService bucketAlgorithmService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userIpAddress = context.Connection.RemoteIpAddress?.ToString();
        if (context.Request.Path.StartsWithSegments("/limited") && !string.IsNullOrEmpty(userIpAddress))
        {
            bucketAlgorithmService?.AddUserWithTokens(userIpAddress);
            var isAllowed = (bool)bucketAlgorithmService?.CheckIfUserHasTokens(userIpAddress);
            if (isAllowed)
            {
                await next(context);
                bucketAlgorithmService?.RemoveUserWithTokens(userIpAddress);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
            return;
        }
        await next(context);
    }
}