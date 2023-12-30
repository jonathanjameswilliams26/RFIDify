using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RFIDify.Api.ExceptionHandlers;

public class DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		logger.LogError(exception, "An unhandled exception occurred while processing the request.");
		
		var response = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "An unhandled exception occurred while processing the request.",
		};
		
		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
		await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
		return true;
	}
}