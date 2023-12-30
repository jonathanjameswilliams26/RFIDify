using RFIDify.Shared.Services;

namespace RFIDify.Spotify.Services;

public interface ISpotifyAccessTokenProvider
{
	Task<string?> GetAccessToken(CancellationToken cancellationToken);
}

public class SpotifyAccessTokenProvider(AppDbContext database, ILogger<SpotifyAccessTokenProvider> logger, ISpotifyAccountsApi spotifyAccountsApi, IDateTimeProvider dateTimeProvider) : ISpotifyAccessTokenProvider
{
	public async Task<string?> GetAccessToken(CancellationToken cancellationToken)
	{
		logger.LogDebug("Spotify Access Token Provider: Getting access token...");

		var accessToken = await database.SpotifyAccessToken.SingleOrDefaultAsync(cancellationToken);

		if (accessToken is null)
		{
			logger.LogWarning("Spotify Access Token Provider: No access token found");
			return null;
		}
		
		// Refresh the access token if it's expired
		if (dateTimeProvider.UtcNow >= accessToken.ExpiresAtUtc)
		{
			var credentials = await database.SpotifyCredentials.SingleAsync(cancellationToken);
			var newAccessToken = await spotifyAccountsApi.RefreshAccessToken(credentials, accessToken.RefreshToken, cancellationToken);
			accessToken.AccessToken = newAccessToken.AccessToken;
			accessToken.ExpiresAtUtc = newAccessToken.ExpiresAtUtc;
			await database.SaveChangesAsync(cancellationToken);
		}

		logger.LogDebug("Spotify Access Token Provider: Successfully got access token");
		return accessToken.AccessToken;
	}
}