namespace RFIDify.Spotify.DataTypes;

public class SpotifyAccessToken
{
	public required string RefreshToken { get; init; }
	public required string AccessToken { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}