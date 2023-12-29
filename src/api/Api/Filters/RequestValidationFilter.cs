namespace RFIDify.Api.Filters;

public class RequestValidationFilter<TRequest> : IEndpointFilter
{
	private readonly IValidator<TRequest> validator;
	private readonly ILogger<RequestValidationFilter<TRequest>> logger;

	public RequestValidationFilter(IValidator<TRequest> validator, ILogger<RequestValidationFilter<TRequest>> logger)
    {
		this.validator = validator;
		this.logger = logger;
	}

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var request = context.Arguments.OfType<TRequest>().Single();
		var path = context.HttpContext.Request.Path;

		logger.LogInformation("{Path} Validating...", path);

		var result = await validator.ValidateAsync(request);
		if (result.IsValid)
		{
			logger.LogInformation("{Path} Request validation successful", path);
			return await next(context);
		}

		var errors = result.ToDictionary();
		logger.LogWarning("{Path} Request validation failed. Reason: {@Errors}", path, errors);
		return TypedResults.ValidationProblem(result.ToDictionary());
	}
}

public static class RequestValidationFilterExtensions
{
	public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder) => builder
		.AddEndpointFilter<RequestValidationFilter<TRequest>>()
		.ProducesValidationProblem();
}