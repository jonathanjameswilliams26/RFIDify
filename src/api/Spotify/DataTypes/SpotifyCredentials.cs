namespace RFIDify.Spotify.DataTypes;

public class SpotifyCredentials
{
	public required string ClientId { get; init; }
	public required string ClientSecret { get; init; }
	public required string State { get; init; }
	public required string RedirectUri { get; init; }
}