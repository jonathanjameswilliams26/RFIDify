namespace RFIDify.Api.Filters;

public class RequestLoggingFilter : IEndpointFilter
{
	private readonly ILogger<RequestLoggingFilter> logger;

	public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger)
    {
		this.logger = logger;
	}

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		logger.LogInformation("HTTP {Method} {Path} starting...", context.HttpContext.Request.Method, context.HttpContext.Request.Path);
		return next(context);
	}
}
