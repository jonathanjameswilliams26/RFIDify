using RFIDify.RFID.Endpoints;
using RFIDify.Spotify.Endpoints;

namespace RFIDify.Api;

public static class Endpoints
{
	public static void UseEndpoints(this WebApplication app)
	{
		var endpoints = app.MapGroup("")
			.WithOpenApi()
			.AddEndpointFilter<RequestLoggingFilter>();

		endpoints.MapRFIDEndpoints();
		endpoints.MapSpotifyEndpoints();
	}

	private static void MapRFIDEndpoints(this IEndpointRouteBuilder app)
	{
		var endpoints = app.MapGroup("/rfid")
			.WithTags("RFID");

		endpoints.MapGetRFIDs();
		endpoints.MapCreateOrUpdateRFID();
		endpoints.MapScanRFID();
	}

	private static void MapSpotifyEndpoints(this IEndpointRouteBuilder app)
	{
		var endpoints = app.MapGroup("/spotify")
			.WithTags("Spotify");

		endpoints.MapSetSpotifyCredentials();
		endpoints.MapSpotifyAuthorisationCallback();
	}
}
