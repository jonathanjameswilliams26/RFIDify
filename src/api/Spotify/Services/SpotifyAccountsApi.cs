using RFIDify.Shared.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace RFIDify.Spotify.Services;

public interface ISpotifyAccountsApi
{
	Uri GenerateAuthorisationUri(SpotifyCredentials credentials);
	Task<SpotifyAccessToken> GetAccessToken(SpotifyCredentials credentials, string code, CancellationToken cancellationToken);
	Task<SpotifyAccessToken> RefreshAccessToken(SpotifyCredentials credentials, string refreshToken, CancellationToken cancellationToken);
}

public class SpotifyAccountsApi(HttpClient httpClient, IDateTimeProvider dateTimeProvider, ILogger<SpotifyAccountsApi> logger) : ISpotifyAccountsApi
{
	public Uri GenerateAuthorisationUri(SpotifyCredentials credentials)
	{
		var uri = httpClient.BaseAddress!.ToString().Replace("/api", "/authorize");

		var builder = new UriBuilder(uri)
		{
			Query = QueryString.Create(new Dictionary<string, string?>
			{
				["client_id"] = credentials.ClientId,
				["response_type"] = "code",
				["redirect_uri"] = credentials.RedirectUri,
				["state"] = credentials.State,
				["scope"] = "user-read-playback-state user-modify-playback-state",
				["show_dialog"] = "true"
			}).ToString()
		};

		return builder.Uri;
	}

	public async Task<SpotifyAccessToken> GetAccessToken(SpotifyCredentials credentials, string code, CancellationToken cancellationToken)
	{
		logger.LogInformation("Spotify Accounts API: Exchanging code for access token...");

		var request = new Dictionary<string, string>
		{
			["grant_type"] = "authorization_code",
			["code"] = code,
			["redirect_uri"] = credentials.RedirectUri
		};

		var response = await SendRequest<AccessTokenWithRefreshTokenResponse>(credentials, request, cancellationToken);
		logger.LogInformation("Spotify Accounts API: Successfully exchanged code for access token");
		return new SpotifyAccessToken
		{
			AccessToken = response.AccessToken,
			ExpiresAtUtc = CalculateExpiresAtUtc(response.ExpiresIn),
			RefreshToken = response.RefreshToken
		};
	}

	public async Task<SpotifyAccessToken> RefreshAccessToken(SpotifyCredentials credentials, string refreshToken, CancellationToken cancellationToken)
	{
		logger.LogInformation("Spotify Accounts API: Refreshing access token...");

		var request = new Dictionary<string, string>
		{
			["grant_type"] = "refresh_token",
			["refresh_token"] = refreshToken
		};

		var response = await SendRequest<AccessTokenResponse>(credentials, request, cancellationToken);
		logger.LogInformation("Spotify Accounts API: Successfully refreshed access token");
		return new SpotifyAccessToken
		{
			AccessToken = response.AccessToken,
			ExpiresAtUtc = CalculateExpiresAtUtc(response.ExpiresIn),
			RefreshToken = refreshToken
		};
	}

	private async Task<T> SendRequest<T>(SpotifyCredentials credentials, Dictionary<string, string> requestBody, CancellationToken cancellationToken)
		where T : AccessTokenResponse
	{
		var clientIdAndSecret = $"{credentials.ClientId}:{credentials.ClientSecret}";
		var encodedClientIdAndSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientIdAndSecret));

		var request = new HttpRequestMessage(HttpMethod.Post, $"{httpClient.BaseAddress}/token");
		request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedClientIdAndSecret);
		request.Content = new FormUrlEncodedContent(requestBody);

		var response = await httpClient.SendAsync(request, cancellationToken);
		var content = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
		return content!;
	}

	private DateTime CalculateExpiresAtUtc(int expiresIn) => dateTimeProvider.UtcNow.AddSeconds(expiresIn);

	private record AccessTokenResponse
	{
		[JsonPropertyName("access_token")]
		public required string AccessToken { get; init; }
		[JsonPropertyName("expires_in")]
		public required int ExpiresIn { get; init; }
	}

	private record AccessTokenWithRefreshTokenResponse : AccessTokenResponse
	{
		[JsonPropertyName("refresh_token")]
		public required string RefreshToken { get; init; }
	}
}