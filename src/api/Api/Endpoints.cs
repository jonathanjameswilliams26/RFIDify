﻿using RFIDify.RFID.Endpoints;

namespace RFIDify.Api;

public static class Endpoints
{
	public static void UseEndpoints(this WebApplication app)
	{
		var endpoints = app.MapGroup("")
			.WithOpenApi();

		endpoints.MapRFIDEndpoints();
	}

	private static void MapRFIDEndpoints(this IEndpointRouteBuilder app)
	{
		var endpoints = app.MapGroup("/rfid")
			.WithTags("RFID");
		
		endpoints.MapCreateOrUpdateRFID();
	}
}