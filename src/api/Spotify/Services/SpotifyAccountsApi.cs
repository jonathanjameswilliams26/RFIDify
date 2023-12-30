namespace RFIDify.Spotify.Services;

public interface ISpotifyAccountsApi
{
	Uri GenerateAuthorisationUri(SpotifyCredentials credentials);
}

public class SpotifyAccountsApi : ISpotifyAccountsApi
{
	private readonly HttpClient httpClient;

	public SpotifyAccountsApi(HttpClient httpClient)
	{
		this.httpClient = httpClient;
	}

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
}
