namespace RFIDify.RFID.Endpoints;

public record ScanRFIDRequest(string RFID);

public class ScanRFIDRequestValidator : AbstractValidator<ScanRFIDRequest>
{
	public ScanRFIDRequestValidator()
	{
		RuleFor(x => x.RFID).NotEmpty();
	}
}

public static class ScanRFID
{
	public static void MapScanRFID(this IEndpointRouteBuilder app) => app
		.MapPost("/scan", Handle)
		.WithSummary("Scan RFID tag and start playing on Spotify")
		.WithRequestValidation<ScanRFIDRequest>();

	private static async Task<Results<Ok, NotFound>> Handle(ScanRFIDRequest request, AppDbContext database, CancellationToken cancellationToken)
	{
		var rfid = await database.RFIDs
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.Id == request.RFID, cancellationToken);

		if (rfid is null)
		{
			return TypedResults.NotFound();
		}

		// TODO: Start playing on Spotify
		return TypedResults.Ok();
	}
}