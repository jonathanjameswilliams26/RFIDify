namespace RFIDify;

public static class ConfigureServices
{
	/// <summary>
	/// Configures the application services.
	/// </summary>
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.AddSwagger();
		builder.AddDatabase();
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
	}

	private static void AddSwagger(this WebApplicationBuilder builder)
	{
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
	}

	private static void AddDatabase(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<AppDbContext>(options =>
		{
			var connectionString = builder.Configuration.GetConnectionString("Default");
			options.UseSqlite(connectionString);
		});
	}
}
