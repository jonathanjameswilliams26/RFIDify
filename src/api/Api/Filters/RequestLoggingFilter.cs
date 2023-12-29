namespace RFIDify.Api.Filters;

public class RequestLoggingFilter(ILogger<RequestLoggingFilter> logger) : IEndpointFilter
{
	public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		logger.LogInformation("HTTP {Method} {Path} starting...", context.HttpContext.Request.Method, context.HttpContext.Request.Path);
		return next(context);
	}
}
