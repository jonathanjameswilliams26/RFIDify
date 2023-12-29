namespace RFIDify;

public static class ConfigureServices
{
	/// <summary>
	/// Configures the application services.
	/// </summary>
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
	}
}
