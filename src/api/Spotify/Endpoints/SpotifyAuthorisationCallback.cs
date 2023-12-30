namespace RFIDify.Spotify.Endpoints;

public record SpotifyAuthorisationCallbackRequest(string? Code, string? State, string? Error);

public static class SpotifyAuthorisationCallback
{
	public static void MapSpotifyAuthorisationCallback(this IEndpointRouteBuilder app) => app
		.MapGet("/authorise/callback", Handle)
		.WithSummary("Callback for Spotify authorisation")
		.WithDescription("This endpoint is called by Spotify after the user has authorised the application to control their Spotify account.");

	private static async Task<Results<Ok, UnauthorizedHttpResult>> Handle([AsParameters]SpotifyAuthorisationCallbackRequest request, AppDbContext database, ILogger<SpotifyAuthorisationCallbackRequest> logger, ISpotifyAccountsApi spotifyAccountsApi, CancellationToken cancellationToken)
	{
		if (request.Error is not null)
		{
			logger.LogWarning("Spotify authorisation callback error: {Error}", request.Error);
			return TypedResults.Unauthorized();
		}

		// Confirm state matches
		var credentials = await database.SpotifyCredentials
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.State == request.State, cancellationToken);
		
		if (credentials is null)
		{
			logger.LogWarning("Spotify authorisation callback state mismatch");
			return TypedResults.Unauthorized();
		}

		// Exchange code for access token
		var accessToken = await spotifyAccountsApi.GetAccessToken(credentials, request.Code!, cancellationToken);
		await database.SpotifyAccessToken.AddAsync(accessToken, cancellationToken);
		await database.SaveChangesAsync(cancellationToken);
		return TypedResults.Ok();
	}
}