using System.Security.Cryptography;

namespace RFIDify.Spotify.Endpoints;

public record SetSpotifyCredentialsRequest(string ClientId, string ClientSecret, string RedirectUri);

public class SetSpotifyCredentialsRequestValidator : AbstractValidator<SetSpotifyCredentialsRequest>
{
	public SetSpotifyCredentialsRequestValidator()
	{
		RuleFor(x => x.ClientId).NotEmpty();
		RuleFor(x => x.ClientSecret).NotEmpty();
		RuleFor(x => x.RedirectUri)
			.NotEmpty()
			.Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
			.WithMessage("Must be a valid Uri");
	}
}

public static class SetSpotifyCredentials
{
	public static void MapSetSpotifyCredentials(this IEndpointRouteBuilder app) => app
		.MapPost("/credentials", Handle)
		.WithSummary("Set Spotify credentials")
		.WithRequestValidation<SetSpotifyCredentialsRequest>();

	private static async Task<RedirectHttpResult> Handle(SetSpotifyCredentialsRequest request, AppDbContext database, ISpotifyAccountsApi spotifyAccountsApi, ILogger<SetSpotifyCredentialsRequest> logger, CancellationToken cancellationToken)
	{
		// Remove any existing credentials
		var existingCredentials = await database.SpotifyCredentials.ToListAsync(cancellationToken);
		var existingTokens = await database.SpotifyAccessToken.ToListAsync(cancellationToken);
		database.SpotifyCredentials.RemoveRange(existingCredentials);
		database.SpotifyAccessToken.RemoveRange(existingTokens);

		// Add new credentials
		var credentials = new SpotifyCredentials
		{
			ClientId = request.ClientId,
			ClientSecret = request.ClientSecret,
			RedirectUri = request.RedirectUri,
			State = RandomNumberGenerator.GetHexString(32)
		};
		await database.SpotifyCredentials.AddAsync(credentials, cancellationToken);
		await database.SaveChangesAsync(cancellationToken);

		// Generate authorisation uri
		var uri = spotifyAccountsApi.GenerateAuthorisationUri(credentials).ToString();
		logger.LogInformation("Generated authorisation uri: {Uri}", uri);
		return TypedResults.Redirect(uri);
	}
}
