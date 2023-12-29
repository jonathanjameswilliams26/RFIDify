namespace RFIDify.RFID.Endpoints;

public static class GetRFIDs
{
	public static void MapGetRFIDs(this IEndpointRouteBuilder app) => app
		.MapGet("/", Handle)
		.WithSummary("Get all RFID tags");

	private static Task<List<RFIDTag>> Handle(AppDbContext database, CancellationToken cancellationToken) => database.RFIDs
		.AsNoTracking()
		.ToListAsync(cancellationToken);
}
