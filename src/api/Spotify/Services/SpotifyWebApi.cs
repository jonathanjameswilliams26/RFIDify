using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace RFIDify.Spotify.Services;

public interface ISpotifyWebApi
{
	Task Play(RFIDTag rfid, CancellationToken cancellationToken);
}

public class SpotifyWebApi(HttpClient httpClient, ILogger<SpotifyWebApi> logger) : ISpotifyWebApi
{
	public async Task Play(RFIDTag rfid, CancellationToken cancellationToken)
	{
		logger.LogInformation("Spotify API: Starting playback for {RFID} - {SpotifyUri}", rfid.Id, rfid.SpotifyUri);

		var isTrack = rfid.SpotifyUri.StartsWith("spotify:track:");

		var requestBody = new StartPlaybackRequest
		{
			ContextUri = isTrack ? null : rfid.SpotifyUri,
			Uris = isTrack ? [rfid.SpotifyUri] : null
		};

		var response = await httpClient.PutAsJsonAsync($"{httpClient.BaseAddress}/me/player/play", requestBody, cancellationToken);

		if (response.IsSuccessStatusCode)
		{
			logger.LogInformation("Spotify API: Successfully started playback for {RFID} - {SpotifyUri}", rfid.Id, rfid.SpotifyUri);
			return;
		}

		logger.LogError("Spotify API: Failed to start playback for {SpotifyUri} - {StatusCode} - {Content}", rfid.SpotifyUri, response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
	}

	private record StartPlaybackRequest
	{
		[JsonPropertyName("context_uri")]
		public string? ContextUri { get; init; }

		[JsonPropertyName("uris")]
		public List<string>? Uris { get; init; }
	}
}

/// <summary>
/// Attaches the Spotify access token to the request.
/// </summary>
public class SpotifyWebApiAccessTokenHandler(ISpotifyAccessTokenProvider spotifyAccessTokenProvider, ILogger<SpotifyWebApiAccessTokenHandler> logger) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		logger.LogDebug("Spotify API: Attaching access token to request...");
		var accessToken = await spotifyAccessTokenProvider.GetAccessToken(cancellationToken);

		if (accessToken is null)
		{
			logger.LogError("Spotify API: No access token found");
			// TODO: return error
			return await base.SendAsync(request, cancellationToken);
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		logger.LogDebug("Spotify API: Successfully attached access token to request");
		return await base.SendAsync(request, cancellationToken);
	}
}