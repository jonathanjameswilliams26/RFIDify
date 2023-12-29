using System.Text.RegularExpressions;

namespace RFIDify.RFID.Endpoints;

public record CreateOrUpdateRFIDRequest(string RFID, string SpotifyUri);

public partial class CreateOrUpdateRFIDRequestValidator : AbstractValidator<CreateOrUpdateRFIDRequest>
{
	[GeneratedRegex("spotify:(playlist|album|track|artist):[a-zA-Z0-9]")]
	private static partial Regex SpotifyUriRegex();

	public CreateOrUpdateRFIDRequestValidator()
	{
		RuleFor(x => x.RFID).NotEmpty();
		RuleFor(x => x.SpotifyUri)
			.NotEmpty()
			.Must(x => SpotifyUriRegex().IsMatch(x))
			.WithMessage("Must in Spotify Uri format. e.g. spotify:track:id");
	}
}

public static class CreateOrUpdateRFID
{
	public static void MapCreateOrUpdateRFID(this IEndpointRouteBuilder app) => app
		.MapPost("/", Handle)
		.WithSummary("Create or update an RFID tag")
		.WithRequestValidation<CreateOrUpdateRFIDRequest>();

	private static async Task<Results<Ok, Created>> Handle(CreateOrUpdateRFIDRequest request, AppDbContext database, CancellationToken cancellationToken)
	{
		var rfid = await database.RFIDs.FirstOrDefaultAsync(x => x.Id == request.RFID, cancellationToken);

		if (rfid is not null)
		{
			rfid.SpotifyUri = request.SpotifyUri;
			await database.SaveChangesAsync(cancellationToken);
			return TypedResults.Ok();
		}

		rfid = new RFIDTag
		{
			Id = request.RFID,
			SpotifyUri = request.SpotifyUri
		};
		await database.RFIDs.AddAsync(rfid, cancellationToken);
		await database.SaveChangesAsync(cancellationToken);
		return TypedResults.Created();
	}
}