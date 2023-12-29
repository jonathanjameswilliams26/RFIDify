using RFIDify.Api.ExceptionHandlers;
using Serilog;

namespace RFIDify;

public static class ConfigureServices
{
	/// <summary>
	/// Configures the application services.
	/// </summary>
	public static void AddServices(this WebApplicationBuilder builder)
	{
		builder.AddSerilog();
		builder.AddSwagger();
		builder.AddDatabase();
		builder.Services.AddValidatorsFromAssemblyContaining<Program>();
		builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
	}

	private static void AddSerilog(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog((context, loggerConfiguration) =>
		{
			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.Enrich.FromLogContext();
		});
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
