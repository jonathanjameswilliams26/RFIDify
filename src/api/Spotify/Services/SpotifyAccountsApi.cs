using RFIDify.Shared.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace RFIDify.Spotify.Services;

public interface ISpotifyAccountsApi
{
	Uri GenerateAuthorisationUri(SpotifyCredentials credentials);
	Task<SpotifyAccessToken> GetAccessToken(SpotifyCredentials credentials, string code, CancellationToken cancellationToken);
}

public class SpotifyAccountsApi(HttpClient httpClient, IDateTimeProvider dateTimeProvider, ILogger<SpotifyAccountsApi> logger) : ISpotifyAccountsApi
{
	public Uri GenerateAuthorisationUri(SpotifyCredentials credentials)
	{
		var builder = new UriBuilder($"{httpClient.BaseAddress}authorize")
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
		logger.LogInformation("Spotify Accounts API: Exchanging code for access token");
		var clientIdAndSecret = $"{credentials.ClientId}:{credentials.ClientSecret}";
		var encodedClientIdAndSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientIdAndSecret));

		var request = new HttpRequestMessage(HttpMethod.Post, $"{httpClient.BaseAddress}api/token")
		{
			Headers =
			{
				Authorization = new AuthenticationHeaderValue("Basic", encodedClientIdAndSecret)
			},
			Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				["grant_type"] = "authorization_code",
				["code"] = code,
				["redirect_uri"] = credentials.RedirectUri
			})
		};

		var response = await httpClient.SendAsync(request, cancellationToken);
		var content = await response.Content.ReadFromJsonAsync<SpotifyAccessTokenResponse>(cancellationToken);
		var accessToken = new SpotifyAccessToken
		{
			AccessToken = content!.AccessToken,
			RefreshToken = content.RefreshToken,
			ExpiresAtUtc = dateTimeProvider.UtcNow.AddSeconds(content.ExpiresIn)
		};

		logger.LogInformation("Spotify Accounts API: Successfully exchanged code for access token");
		return accessToken;
	}

	private record SpotifyAccessTokenResponse
	{
		[JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }
		[JsonPropertyName("refresh_token")]
        public required string RefreshToken { get; init; }
		[JsonPropertyName("expires_in")]
		public required int ExpiresIn { get; init; }
    }
}
